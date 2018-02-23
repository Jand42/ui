// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2014 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}

namespace WebSharper.UI

#nowarn "44" // HTML deprecated

open System
open Microsoft.FSharp.Quotations
open WebSharper
open WebSharper.Web
open WebSharper.Sitelets
open WebSharper.JavaScript
open WebSharper.Core.Resources

[<AbstractClass>]
type Doc() =

    interface IControlBody with
        member this.ReplaceInDom n = X<unit>

    interface INode with
        member this.Write(ctx, w) = this.Write(ctx, w, false)
        member this.IsAttribute = false

    interface IRequiresResources with
        member this.Encode(meta, json) = this.Encode(meta, json)
        member this.Requires(meta) = this.Requires(meta)

    abstract Write : Web.Context * HtmlTextWriter * res: option<Sitelets.Content.RenderedResources> -> unit
    abstract Write : Web.Context * HtmlTextWriter * renderResources: bool -> unit
    abstract HasNonScriptSpecialTags : bool
    abstract Encode : Core.Metadata.Info * Core.Json.Provider -> list<string * Core.Json.Encoded>
    abstract Requires : Core.Metadata.Info -> seq<Core.Metadata.Node>

    default this.Write(ctx: Web.Context, w: HtmlTextWriter, renderResources: bool) =
        let resources =
            if renderResources then
                if this.HasNonScriptSpecialTags then
                    ctx.GetSeparateResourcesAndScripts [this]
                else
                    { Scripts = ctx.GetResourcesAndScripts [this]; Styles = ""; Meta = "" }
                |> Some
            else None
        this.Write(ctx, w, resources)

and ConcreteDoc(dd: DynDoc) =
    inherit Doc()

    override this.Write(ctx, w, res: option<Sitelets.Content.RenderedResources>) =
        match dd with
        | AppendDoc docs ->
            docs |> List.iter (fun d -> d.Write(ctx, w, res))
        | ElemDoc elt ->
            elt.Write(ctx, w, res)
        | EmptyDoc -> ()
        | TextDoc t -> w.WriteEncodedText(t)
        | VerbatimDoc t -> w.Write(t)
        | INodeDoc d -> d.Write(ctx, w)

    override this.HasNonScriptSpecialTags =
        match dd with
        | AppendDoc docs ->
            docs |> List.exists (fun d -> d.HasNonScriptSpecialTags)
        | ElemDoc elt ->
            (elt :> Doc).HasNonScriptSpecialTags
        | _ -> false

    override this.Encode(meta, json) =
        match dd with
        | AppendDoc docs -> docs |> List.collect (fun d -> d.Encode(meta, json))
        | INodeDoc c -> c.Encode(meta, json)
        | ElemDoc elt -> (elt :> IRequiresResources).Encode(meta, json)
        | _ -> []

    override this.Requires(meta) =
        match dd with
        | AppendDoc docs -> docs |> Seq.collect (fun d -> d.Requires(meta))
        | INodeDoc c -> (c :> IRequiresResources).Requires(meta)
        | ElemDoc elt -> (elt :> IRequiresResources).Requires(meta)
        | _ -> Seq.empty

and DynDoc =
    | AppendDoc of list<Doc>
    | ElemDoc of Elt
    | EmptyDoc
    | TextDoc of string
    | VerbatimDoc of string
    | INodeDoc of INode

and HoleName = Replace | Hole

and Elt
    (
        attrs: list<Attr>,
        encode, requires, hasNonScriptSpecialTags,
        write: list<Attr> -> Web.Context -> HtmlTextWriter -> option<Sitelets.Content.RenderedResources> -> unit,
        write': option<list<Attr> -> Web.Context -> HtmlTextWriter -> bool -> unit>
    ) =
    inherit Doc()

    let mutable attrs = attrs

    override this.HasNonScriptSpecialTags = hasNonScriptSpecialTags

    override this.Encode(m, j) = encode m j

    override this.Requires(meta) = requires attrs meta

    override this.Write(ctx, h, res) = write attrs ctx h res

    override this.Write(ctx, h, res) =
        match write' with
        | Some f -> f attrs ctx h res
        | None -> base.Write(ctx, h, res)

    new (tag: string, attrs: list<Attr>, children: list<Doc>) =
        let write attrs (ctx: Web.Context) (w: HtmlTextWriter) (res: option<Sitelets.Content.RenderedResources>) =
            let hole =
                res |> Option.bind (fun res ->
                    let rec findHole a =
                        if obj.ReferenceEquals(a, null) then None else
                        match a with
                        | Attr.SingleAttr (("ws-replace" | "data-replace"), value)
                            when (value = "scripts" || value = "styles" || value = "meta") ->
                            Some (HoleName.Replace, value, res)
                        | Attr.SingleAttr (("ws-hole" | "data-hole"), value)
                            when (value = "scripts" || value = "styles" || value = "meta") ->
                            Some (HoleName.Hole, value, res)
                        | Attr.SingleAttr _ | Attr.DepAttr _ -> None
                        | Attr.AppendAttr attrs -> List.tryPick findHole attrs
                    List.tryPick findHole attrs
                )
            match hole with
            | Some (HoleName.Replace, name, res) -> w.Write(res.[name])
            | Some (HoleName.Hole, name, res) ->
                w.WriteBeginTag(tag)
                attrs |> List.iter (fun a -> a.Write(ctx.Metadata, w, true))
                w.Write(HtmlTextWriter.TagRightChar)
                w.Write(res.[name])
                w.WriteEndTag(tag)
            | None ->
                w.WriteBeginTag(tag)
                attrs |> List.iter (fun a ->
                    if not (obj.ReferenceEquals(a, null))
                    then a.Write(ctx.Metadata, w, false))
                if List.isEmpty children && HtmlTextWriter.IsSelfClosingTag tag then
                    w.Write(HtmlTextWriter.SelfClosingTagEnd)
                else
                    w.Write(HtmlTextWriter.TagRightChar)
                    children |> List.iter (fun e -> e.Write(ctx, w, res))
                    w.WriteEndTag(tag)

        let hasNonScriptSpecialTags =
            let rec testAttr a =
                if obj.ReferenceEquals(a, null) then false else
                match a with
                | Attr.AppendAttr attrs -> List.exists testAttr attrs
                | Attr.SingleAttr (("ws-replace" | "ws-hole" | "data-replace" | "data-hole"), ("styles" | "meta")) -> true
                | Attr.SingleAttr _
                | Attr.DepAttr _ -> false
            (attrs |> List.exists testAttr)
            || (children |> List.exists (fun d -> d.HasNonScriptSpecialTags))

        let encode meta json =
            children |> List.collect (fun e -> (e :> IRequiresResources).Encode(meta, json))

        let requires attrs meta =
            Seq.append
                (attrs |> Seq.collect (fun a ->
                    if obj.ReferenceEquals(a, null)
                    then Seq.empty
                    else (a :> IRequiresResources).Requires(meta)))
                (children |> Seq.collect (fun e -> (e :> IRequiresResources).Requires(meta)))

        Elt(attrs, encode, requires, hasNonScriptSpecialTags, write, None)

    member this.OnImpl(ev, asm, cb) =
        attrs <- Attr.HandlerImpl ev asm cb :: attrs
        this

    member this.On(ev, [<JavaScript>] cb) =
        this.OnImpl(ev, System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)

    member this.OnLinq(ev, cb) =
        attrs <- Attr.HandlerLinq ev cb :: attrs
        this

    member this.WithAttrs(a) =
        attrs <- a @ attrs
        this

    // {{ event
    member this.OnAbort([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.UIEvent -> unit>) = this.OnImpl("abort", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnAfterPrint([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("afterprint", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnAnimationEnd([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("animationend", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnAnimationIteration([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("animationiteration", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnAnimationStart([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("animationstart", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnAudioProcess([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("audioprocess", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnBeforePrint([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("beforeprint", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnBeforeUnload([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("beforeunload", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnBeginEvent([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("beginEvent", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnBlocked([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("blocked", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnBlur([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.FocusEvent -> unit>) = this.OnImpl("blur", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnCached([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("cached", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnCanPlay([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("canplay", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnCanPlayThrough([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("canplaythrough", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("change", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnChargingChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("chargingchange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnChargingTimeChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("chargingtimechange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnChecking([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("checking", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnClick([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("click", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnClose([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("close", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnComplete([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("complete", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnCompositionEnd([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.CompositionEvent -> unit>) = this.OnImpl("compositionend", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnCompositionStart([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.CompositionEvent -> unit>) = this.OnImpl("compositionstart", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnCompositionUpdate([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.CompositionEvent -> unit>) = this.OnImpl("compositionupdate", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnContextMenu([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("contextmenu", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnCopy([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("copy", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnCut([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("cut", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDblClick([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("dblclick", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDeviceLight([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("devicelight", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDeviceMotion([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("devicemotion", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDeviceOrientation([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("deviceorientation", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDeviceProximity([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("deviceproximity", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDischargingTimeChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("dischargingtimechange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMActivate([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.UIEvent -> unit>) = this.OnImpl("DOMActivate", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMAttributeNameChanged([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("DOMAttributeNameChanged", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMAttrModified([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = this.OnImpl("DOMAttrModified", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMCharacterDataModified([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = this.OnImpl("DOMCharacterDataModified", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMContentLoaded([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("DOMContentLoaded", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMElementNameChanged([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("DOMElementNameChanged", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMNodeInserted([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = this.OnImpl("DOMNodeInserted", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMNodeInsertedIntoDocument([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = this.OnImpl("DOMNodeInsertedIntoDocument", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMNodeRemoved([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = this.OnImpl("DOMNodeRemoved", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMNodeRemovedFromDocument([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = this.OnImpl("DOMNodeRemovedFromDocument", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDOMSubtreeModified([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = this.OnImpl("DOMSubtreeModified", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDownloading([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("downloading", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDrag([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("drag", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDragEnd([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("dragend", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDragEnter([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("dragenter", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDragLeave([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("dragleave", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDragOver([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("dragover", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDragStart([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("dragstart", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDrop([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("drop", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnDurationChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("durationchange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnEmptied([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("emptied", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnEnded([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("ended", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnEndEvent([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("endEvent", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnError([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("error", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnFocus([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.FocusEvent -> unit>) = this.OnImpl("focus", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnFullScreenChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("fullscreenchange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnFullScreenError([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("fullscreenerror", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnGamepadConnected([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("gamepadconnected", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnGamepadDisconnected([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("gamepaddisconnected", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnHashChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("hashchange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnInput([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("input", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnInvalid([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("invalid", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnKeyDown([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.KeyboardEvent -> unit>) = this.OnImpl("keydown", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnKeyPress([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.KeyboardEvent -> unit>) = this.OnImpl("keypress", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnKeyUp([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.KeyboardEvent -> unit>) = this.OnImpl("keyup", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnLanguageChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("languagechange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnLevelChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("levelchange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnLoad([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.UIEvent -> unit>) = this.OnImpl("load", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnLoadedData([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("loadeddata", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnLoadedMetadata([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("loadedmetadata", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnLoadEnd([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("loadend", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnLoadStart([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("loadstart", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnMessage([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("message", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnMouseDown([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("mousedown", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnMouseEnter([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("mouseenter", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnMouseLeave([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("mouseleave", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnMouseMove([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("mousemove", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnMouseOut([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("mouseout", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnMouseOver([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("mouseover", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnMouseUp([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("mouseup", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnNoUpdate([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("noupdate", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnObsolete([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("obsolete", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnOffline([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("offline", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnOnline([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("online", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnOpen([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("open", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnOrientationChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("orientationchange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPageHide([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("pagehide", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPageShow([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("pageshow", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPaste([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("paste", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPause([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("pause", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPlay([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("play", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPlaying([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("playing", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPointerLockChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("pointerlockchange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPointerLockError([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("pointerlockerror", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnPopState([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("popstate", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnProgress([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("progress", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnRateChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("ratechange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnReadyStateChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("readystatechange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnRepeatEvent([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("repeatEvent", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnReset([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("reset", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnResize([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.UIEvent -> unit>) = this.OnImpl("resize", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnScroll([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.UIEvent -> unit>) = this.OnImpl("scroll", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSeeked([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("seeked", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSeeking([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("seeking", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSelect([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.UIEvent -> unit>) = this.OnImpl("select", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnShow([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = this.OnImpl("show", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnStalled([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("stalled", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnStorage([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("storage", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSubmit([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("submit", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSuccess([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("success", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSuspend([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("suspend", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSVGAbort([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("SVGAbort", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSVGError([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("SVGError", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSVGLoad([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("SVGLoad", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSVGResize([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("SVGResize", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSVGScroll([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("SVGScroll", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSVGUnload([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("SVGUnload", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnSVGZoom([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("SVGZoom", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTimeOut([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("timeout", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTimeUpdate([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("timeupdate", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTouchCancel([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("touchcancel", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTouchEnd([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("touchend", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTouchEnter([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("touchenter", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTouchLeave([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("touchleave", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTouchMove([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("touchmove", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTouchStart([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("touchstart", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnTransitionEnd([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("transitionend", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnUnload([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.UIEvent -> unit>) = this.OnImpl("unload", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnUpdateReady([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("updateready", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnUpgradeNeeded([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("upgradeneeded", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnUserProximity([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("userproximity", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnVersionChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("versionchange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnVisibilityChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("visibilitychange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnVolumeChange([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("volumechange", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnWaiting([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.Event -> unit>) = this.OnImpl("waiting", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    member this.OnWheel([<JavaScript; ReflectedDefinition>] cb: Expr<Dom.Element -> Dom.WheelEvent -> unit>) = this.OnImpl("wheel", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, cb)
    // }}

and [<RequireQualifiedAccess; JavaScript false>] TemplateHole =
    | Elt of name: string * fillWith: Doc
    | Text of name: string * fillWith: string
    | TextView of name: string * fillWith: View<string>
    | Attribute of name: string * fillWith: Attr
    | Event of name: string * fillWith: (Element -> Dom.Event -> unit)
    | EventQ of name: string * isGenerated: bool * fillWith: Expr<Element -> Dom.Event -> unit>
    | AfterRender of name: string * fillWith: (Element -> unit)
    | VarStr of name: string * fillWith: Var<string>
    | VarBool of name: string * fillWith: Var<bool>
    | VarInt of name: string * fillWith: Var<Client.CheckedInput<int>>
    | VarIntUnchecked of name: string * fillWith: Var<int>
    | VarFloat of name: string * fillWith: Var<Client.CheckedInput<float>>
    | VarFloatUnchecked of name: string * fillWith: Var<float>

    [<Inline>]
    static member NewActionEvent<'T when 'T :> Dom.Event>(name: string, f: Action<Element, 'T>) =
        Event(name, fun el ev -> f.Invoke(el, downcast ev))

    [<Inline "$x.$0">]
    static member Name x =
        match x with
        | TemplateHole.Elt (name, _)
        | TemplateHole.Text (name, _)
        | TemplateHole.TextView (name, _)
        | TemplateHole.VarStr (name, _)
        | TemplateHole.VarBool (name, _)
        | TemplateHole.VarInt (name, _)
        | TemplateHole.VarIntUnchecked (name, _)
        | TemplateHole.VarFloat (name, _)
        | TemplateHole.VarFloatUnchecked (name, _)
        | TemplateHole.Event (name, _)
        | TemplateHole.EventQ (name, _, _)
        | TemplateHole.AfterRender (name, _)
        | TemplateHole.Attribute (name, _) -> name

type Doc with

    static member ListOfSeq (s: seq<'T>) =
        match s with
        | null -> []
        | s -> List.ofSeq s

    static member ToMixedDoc (o: obj) =
        match o with
        | :? Doc as d -> d
        | :? INode as n -> Doc.OfINode n
        | :? Expr<#IControlBody> as e -> Doc.ClientSide e
        | :? string as t -> Doc.TextNode t
        | null -> Doc.Empty
        | o -> Doc.TextNode (string o)

    static member Element (tagname: string) (attrs: seq<Attr>) (children: seq<Doc>) =
        Elt (tagname, Doc.ListOfSeq attrs, Doc.ListOfSeq children)

    static member ElementMixed (tagname: string) (nodes: seq<obj>) =
        let attrs = ResizeArray()
        let children = ResizeArray()
        for n in nodes do
            match n with
            | :? Attr as a -> attrs.Add a
            | o -> children.Add (Doc.ToMixedDoc o)
        Doc.Element tagname attrs children 

    static member SvgElement (tagname: string) (attrs: seq<Attr>) (children: seq<Doc>) =
        Elt (tagname, Doc.ListOfSeq attrs, Doc.ListOfSeq children)

    static member SvgElementMixed (tagname: string) (nodes: seq<obj>) =
        Doc.ElementMixed tagname nodes

    static member Empty = ConcreteDoc(EmptyDoc) :> Doc

    static member Append d1 d2 = ConcreteDoc(AppendDoc [ d1; d2 ]) :> Doc

    static member Concat (docs: seq<Doc>) = ConcreteDoc(AppendDoc (Doc.ListOfSeq docs)) :> Doc

    static member ConcatMixed ([<ParamArray>] docs: obj[]) = Doc.Concat (Seq.map Doc.ToMixedDoc docs)

    static member TextNode t = ConcreteDoc(TextDoc t) :> Doc

    static member ClientSideImpl(expr: Expr<#IControlBody>) =
        ConcreteDoc(INodeDoc (new Web.InlineControl<_>(expr))) :> Doc

    static member ClientSide([<JavaScript>] expr: Expr<#IControlBody>) =
        Doc.ClientSideImpl expr

    static member ClientSideLinq (expr: System.Linq.Expressions.Expression<System.Func<IControlBody>>) =
        ConcreteDoc(INodeDoc (new Web.CSharpInlineControl(expr))) :> Doc

    static member Verbatim t = ConcreteDoc(VerbatimDoc t) :> Doc

    static member OfINode n = ConcreteDoc(INodeDoc n) :> Doc
