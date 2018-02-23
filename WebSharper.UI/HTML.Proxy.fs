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

namespace WebSharper.UI.Client

#nowarn "44" // HTML deprecated

open Microsoft.FSharp.Quotations
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI

[<Proxy(typeof<Html.on>)>]
type private onProxy =
    // {{ event
    [<JavaScript; Inline>]
    static member abort (f: Expr<Dom.Element -> Dom.UIEvent -> unit>) = AttrProxy.HandlerImpl "abort" f
    [<JavaScript; Inline>]
    static member afterPrint (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "afterprint" f
    [<JavaScript; Inline>]
    static member animationEnd (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "animationend" f
    [<JavaScript; Inline>]
    static member animationIteration (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "animationiteration" f
    [<JavaScript; Inline>]
    static member animationStart (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "animationstart" f
    [<JavaScript; Inline>]
    static member audioProcess (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "audioprocess" f
    [<JavaScript; Inline>]
    static member beforePrint (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "beforeprint" f
    [<JavaScript; Inline>]
    static member beforeUnload (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "beforeunload" f
    [<JavaScript; Inline>]
    static member beginEvent (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "beginEvent" f
    [<JavaScript; Inline>]
    static member blocked (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "blocked" f
    [<JavaScript; Inline>]
    static member blur (f: Expr<Dom.Element -> Dom.FocusEvent -> unit>) = AttrProxy.HandlerImpl "blur" f
    [<JavaScript; Inline>]
    static member cached (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "cached" f
    [<JavaScript; Inline>]
    static member canPlay (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "canplay" f
    [<JavaScript; Inline>]
    static member canPlayThrough (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "canplaythrough" f
    [<JavaScript; Inline>]
    static member change (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "change" f
    [<JavaScript; Inline>]
    static member chargingChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "chargingchange" f
    [<JavaScript; Inline>]
    static member chargingTimeChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "chargingtimechange" f
    [<JavaScript; Inline>]
    static member checking (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "checking" f
    [<JavaScript; Inline>]
    static member click (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "click" f
    [<JavaScript; Inline>]
    static member close (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "close" f
    [<JavaScript; Inline>]
    static member complete (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "complete" f
    [<JavaScript; Inline>]
    static member compositionEnd (f: Expr<Dom.Element -> Dom.CompositionEvent -> unit>) = AttrProxy.HandlerImpl "compositionend" f
    [<JavaScript; Inline>]
    static member compositionStart (f: Expr<Dom.Element -> Dom.CompositionEvent -> unit>) = AttrProxy.HandlerImpl "compositionstart" f
    [<JavaScript; Inline>]
    static member compositionUpdate (f: Expr<Dom.Element -> Dom.CompositionEvent -> unit>) = AttrProxy.HandlerImpl "compositionupdate" f
    [<JavaScript; Inline>]
    static member contextMenu (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "contextmenu" f
    [<JavaScript; Inline>]
    static member copy (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "copy" f
    [<JavaScript; Inline>]
    static member cut (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "cut" f
    [<JavaScript; Inline>]
    static member dblClick (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "dblclick" f
    [<JavaScript; Inline>]
    static member deviceLight (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "devicelight" f
    [<JavaScript; Inline>]
    static member deviceMotion (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "devicemotion" f
    [<JavaScript; Inline>]
    static member deviceOrientation (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "deviceorientation" f
    [<JavaScript; Inline>]
    static member deviceProximity (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "deviceproximity" f
    [<JavaScript; Inline>]
    static member dischargingTimeChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "dischargingtimechange" f
    [<JavaScript; Inline>]
    static member DOMActivate (f: Expr<Dom.Element -> Dom.UIEvent -> unit>) = AttrProxy.HandlerImpl "DOMActivate" f
    [<JavaScript; Inline>]
    static member DOMAttributeNameChanged (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "DOMAttributeNameChanged" f
    [<JavaScript; Inline>]
    static member DOMAttrModified (f: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = AttrProxy.HandlerImpl "DOMAttrModified" f
    [<JavaScript; Inline>]
    static member DOMCharacterDataModified (f: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = AttrProxy.HandlerImpl "DOMCharacterDataModified" f
    [<JavaScript; Inline>]
    static member DOMContentLoaded (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "DOMContentLoaded" f
    [<JavaScript; Inline>]
    static member DOMElementNameChanged (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "DOMElementNameChanged" f
    [<JavaScript; Inline>]
    static member DOMNodeInserted (f: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = AttrProxy.HandlerImpl "DOMNodeInserted" f
    [<JavaScript; Inline>]
    static member DOMNodeInsertedIntoDocument (f: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = AttrProxy.HandlerImpl "DOMNodeInsertedIntoDocument" f
    [<JavaScript; Inline>]
    static member DOMNodeRemoved (f: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = AttrProxy.HandlerImpl "DOMNodeRemoved" f
    [<JavaScript; Inline>]
    static member DOMNodeRemovedFromDocument (f: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = AttrProxy.HandlerImpl "DOMNodeRemovedFromDocument" f
    [<JavaScript; Inline>]
    static member DOMSubtreeModified (f: Expr<Dom.Element -> Dom.MutationEvent -> unit>) = AttrProxy.HandlerImpl "DOMSubtreeModified" f
    [<JavaScript; Inline>]
    static member downloading (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "downloading" f
    [<JavaScript; Inline>]
    static member drag (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "drag" f
    [<JavaScript; Inline>]
    static member dragEnd (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "dragend" f
    [<JavaScript; Inline>]
    static member dragEnter (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "dragenter" f
    [<JavaScript; Inline>]
    static member dragLeave (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "dragleave" f
    [<JavaScript; Inline>]
    static member dragOver (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "dragover" f
    [<JavaScript; Inline>]
    static member dragStart (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "dragstart" f
    [<JavaScript; Inline>]
    static member drop (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "drop" f
    [<JavaScript; Inline>]
    static member durationChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "durationchange" f
    [<JavaScript; Inline>]
    static member emptied (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "emptied" f
    [<JavaScript; Inline>]
    static member ended (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "ended" f
    [<JavaScript; Inline>]
    static member endEvent (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "endEvent" f
    [<JavaScript; Inline>]
    static member error (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "error" f
    [<JavaScript; Inline>]
    static member focus (f: Expr<Dom.Element -> Dom.FocusEvent -> unit>) = AttrProxy.HandlerImpl "focus" f
    [<JavaScript; Inline>]
    static member fullScreenChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "fullscreenchange" f
    [<JavaScript; Inline>]
    static member fullScreenError (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "fullscreenerror" f
    [<JavaScript; Inline>]
    static member gamepadConnected (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "gamepadconnected" f
    [<JavaScript; Inline>]
    static member gamepadDisconnected (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "gamepaddisconnected" f
    [<JavaScript; Inline>]
    static member hashChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "hashchange" f
    [<JavaScript; Inline>]
    static member input (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "input" f
    [<JavaScript; Inline>]
    static member invalid (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "invalid" f
    [<JavaScript; Inline>]
    static member keyDown (f: Expr<Dom.Element -> Dom.KeyboardEvent -> unit>) = AttrProxy.HandlerImpl "keydown" f
    [<JavaScript; Inline>]
    static member keyPress (f: Expr<Dom.Element -> Dom.KeyboardEvent -> unit>) = AttrProxy.HandlerImpl "keypress" f
    [<JavaScript; Inline>]
    static member keyUp (f: Expr<Dom.Element -> Dom.KeyboardEvent -> unit>) = AttrProxy.HandlerImpl "keyup" f
    [<JavaScript; Inline>]
    static member languageChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "languagechange" f
    [<JavaScript; Inline>]
    static member levelChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "levelchange" f
    [<JavaScript; Inline>]
    static member load (f: Expr<Dom.Element -> Dom.UIEvent -> unit>) = AttrProxy.HandlerImpl "load" f
    [<JavaScript; Inline>]
    static member loadedData (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "loadeddata" f
    [<JavaScript; Inline>]
    static member loadedMetadata (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "loadedmetadata" f
    [<JavaScript; Inline>]
    static member loadEnd (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "loadend" f
    [<JavaScript; Inline>]
    static member loadStart (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "loadstart" f
    [<JavaScript; Inline>]
    static member message (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "message" f
    [<JavaScript; Inline>]
    static member mouseDown (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "mousedown" f
    [<JavaScript; Inline>]
    static member mouseEnter (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "mouseenter" f
    [<JavaScript; Inline>]
    static member mouseLeave (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "mouseleave" f
    [<JavaScript; Inline>]
    static member mouseMove (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "mousemove" f
    [<JavaScript; Inline>]
    static member mouseOut (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "mouseout" f
    [<JavaScript; Inline>]
    static member mouseOver (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "mouseover" f
    [<JavaScript; Inline>]
    static member mouseUp (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "mouseup" f
    [<JavaScript; Inline>]
    static member noUpdate (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "noupdate" f
    [<JavaScript; Inline>]
    static member obsolete (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "obsolete" f
    [<JavaScript; Inline>]
    static member offline (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "offline" f
    [<JavaScript; Inline>]
    static member online (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "online" f
    [<JavaScript; Inline>]
    static member ``open`` (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "open" f
    [<JavaScript; Inline>]
    static member orientationChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "orientationchange" f
    [<JavaScript; Inline>]
    static member pageHide (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "pagehide" f
    [<JavaScript; Inline>]
    static member pageShow (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "pageshow" f
    [<JavaScript; Inline>]
    static member paste (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "paste" f
    [<JavaScript; Inline>]
    static member pause (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "pause" f
    [<JavaScript; Inline>]
    static member play (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "play" f
    [<JavaScript; Inline>]
    static member playing (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "playing" f
    [<JavaScript; Inline>]
    static member pointerLockChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "pointerlockchange" f
    [<JavaScript; Inline>]
    static member pointerLockError (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "pointerlockerror" f
    [<JavaScript; Inline>]
    static member popState (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "popstate" f
    [<JavaScript; Inline>]
    static member progress (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "progress" f
    [<JavaScript; Inline>]
    static member rateChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "ratechange" f
    [<JavaScript; Inline>]
    static member readyStateChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "readystatechange" f
    [<JavaScript; Inline>]
    static member repeatEvent (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "repeatEvent" f
    [<JavaScript; Inline>]
    static member reset (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "reset" f
    [<JavaScript; Inline>]
    static member resize (f: Expr<Dom.Element -> Dom.UIEvent -> unit>) = AttrProxy.HandlerImpl "resize" f
    [<JavaScript; Inline>]
    static member scroll (f: Expr<Dom.Element -> Dom.UIEvent -> unit>) = AttrProxy.HandlerImpl "scroll" f
    [<JavaScript; Inline>]
    static member seeked (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "seeked" f
    [<JavaScript; Inline>]
    static member seeking (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "seeking" f
    [<JavaScript; Inline>]
    static member select (f: Expr<Dom.Element -> Dom.UIEvent -> unit>) = AttrProxy.HandlerImpl "select" f
    [<JavaScript; Inline>]
    static member show (f: Expr<Dom.Element -> Dom.MouseEvent -> unit>) = AttrProxy.HandlerImpl "show" f
    [<JavaScript; Inline>]
    static member stalled (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "stalled" f
    [<JavaScript; Inline>]
    static member storage (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "storage" f
    [<JavaScript; Inline>]
    static member submit (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "submit" f
    [<JavaScript; Inline>]
    static member success (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "success" f
    [<JavaScript; Inline>]
    static member suspend (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "suspend" f
    [<JavaScript; Inline>]
    static member SVGAbort (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "SVGAbort" f
    [<JavaScript; Inline>]
    static member SVGError (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "SVGError" f
    [<JavaScript; Inline>]
    static member SVGLoad (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "SVGLoad" f
    [<JavaScript; Inline>]
    static member SVGResize (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "SVGResize" f
    [<JavaScript; Inline>]
    static member SVGScroll (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "SVGScroll" f
    [<JavaScript; Inline>]
    static member SVGUnload (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "SVGUnload" f
    [<JavaScript; Inline>]
    static member SVGZoom (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "SVGZoom" f
    [<JavaScript; Inline>]
    static member timeOut (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "timeout" f
    [<JavaScript; Inline>]
    static member timeUpdate (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "timeupdate" f
    [<JavaScript; Inline>]
    static member touchCancel (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "touchcancel" f
    [<JavaScript; Inline>]
    static member touchEnd (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "touchend" f
    [<JavaScript; Inline>]
    static member touchEnter (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "touchenter" f
    [<JavaScript; Inline>]
    static member touchLeave (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "touchleave" f
    [<JavaScript; Inline>]
    static member touchMove (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "touchmove" f
    [<JavaScript; Inline>]
    static member touchStart (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "touchstart" f
    [<JavaScript; Inline>]
    static member transitionEnd (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "transitionend" f
    [<JavaScript; Inline>]
    static member unload (f: Expr<Dom.Element -> Dom.UIEvent -> unit>) = AttrProxy.HandlerImpl "unload" f
    [<JavaScript; Inline>]
    static member updateReady (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "updateready" f
    [<JavaScript; Inline>]
    static member upgradeNeeded (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "upgradeneeded" f
    [<JavaScript; Inline>]
    static member userProximity (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "userproximity" f
    [<JavaScript; Inline>]
    static member versionChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "versionchange" f
    [<JavaScript; Inline>]
    static member visibilityChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "visibilitychange" f
    [<JavaScript; Inline>]
    static member volumeChange (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "volumechange" f
    [<JavaScript; Inline>]
    static member waiting (f: Expr<Dom.Element -> Dom.Event -> unit>) = AttrProxy.HandlerImpl "waiting" f
    [<JavaScript; Inline>]
    static member wheel (f: Expr<Dom.Element -> Dom.WheelEvent -> unit>) = AttrProxy.HandlerImpl "wheel" f
    // }}
