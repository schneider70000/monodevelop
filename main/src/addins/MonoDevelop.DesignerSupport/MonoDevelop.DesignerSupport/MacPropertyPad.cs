//
// PropertyPad.cs: The pad that holds the MD property grid. Can also 
//     hold custom grid widgets.
//
// Authors:
//   Michael Hutchinson <m.j.hutchinson@gmail.com>
//
// Copyright (C) 2006 Michael Hutchinson
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System.Linq;
using System;
using pg = MonoDevelop.Components.PropertyGrid;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Commands;
using AppKit;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Components.Docking;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Commands;

namespace MonoDevelop.DesignerSupport
{
	[System.ComponentModel.Category ("MonoDevelop.Components")]
	[System.ComponentModel.ToolboxItem (true)]
	class MacPropertyGrid : NSStackView
	{

		public MacPropertyGrid ()
		{
			Orientation = NSUserInterfaceLayoutOrientation.Vertical;
			TranslatesAutoresizingMaskIntoConstraints = false;
		}

		internal void SetToolbarProvider (MacDockToolbarProvider toolbarProvider)
		{
			throw new NotImplementedException ();
		}
	}

	public class MacPropertyPad : PadContent, IPropertyPad
	{
		MacPropertyGrid grid;
		MacInvisibleFrame frame;
		bool customWidget;
		MacDockToolbarProvider toolbarProvider = new MacDockToolbarProvider ();
		IPadWindow container;
		internal object CommandRouteOrigin { get; set; }

		public MacPropertyPad ()
		{
			grid = new MacPropertyGrid ();
			frame = new MacInvisibleFrame ();
			frame.AddSubview (grid);

			var toolbar = container.GetToolbar (DockPositionType.Top) as MacDockItemToolbar;

			toolbarProvider.Attach (toolbar); ;
			grid.SetToolbarProvider (toolbarProvider);
			this.container = container;
			DesignerSupport.Service.SetPad (this);
		}

		protected override void Initialize (IPadWindow window)
		{
			base.Initialize (window);
		}

		object ICommandDelegator.GetDelegatedCommandTarget ()
		{
			// Route the save command to the object for which we are inspecting the properties,
			// so pressing the Save shortcut when doing changes in the property pad will save
			// the document we are changing
			if (IdeApp.CommandService.CurrentCommand == IdeApp.CommandService.GetCommand (FileCommands.Save))
				return CommandRouteOrigin;
			else
				return null;
		}


		#region AbstractPadContent implementations

		public override Control Control {
			get { return frame; }
		}

		object IPropertyPad.CommandRouteOrigin { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }

		public IPadWindow PadWindow => throw new NotImplementedException ();

		public pg.IPropertyGrid PropertyGrid => throw new NotImplementedException ();

		public override void Dispose()
		{
			DesignerSupport.Service.SetPad (null);
			base.Dispose ();
		}

		public void UseCustomWidget (object widget)
		{
			throw new NotImplementedException ();
		}

		public void BlankPad ()
		{
			throw new NotImplementedException ();
		}

		#endregion
	}

	class MacDockItemToolbar : IDockItemToolbar
	{
		DockItem parentItem;
		AppKit.NSView frame;
		NSBox box;
		DockPositionType position;
		bool empty = true;

		internal MacDockItemToolbar (DockItem parentItem, DockPositionType position)
		{
			this.parentItem = parentItem;
			this.position = position;
			UpdateAccessibilityLabel ();
		}

		public DockItem DockItem {
			get { return parentItem; }
		}

		internal void UpdateAccessibilityLabel ()
		{
			string name = "";
			switch (position) {
			case DockPositionType.Bottom:
				name = Core.GettextCatalog.GetString ("Bottom {0} pad toolbar", parentItem.Label);
				break;

			case DockPositionType.Left:
				name = Core.GettextCatalog.GetString ("Left {0} pad toolbar", parentItem.Label);
				break;

			case DockPositionType.Right:
				name = Core.GettextCatalog.GetString ("Right {0} pad toolbar", parentItem.Label);
				break;

			case DockPositionType.Top:
				name = Core.GettextCatalog.GetString ("Top {0} pad toolbar", parentItem.Label);
				break;
			}

			//box.Accessible.SetCommonAttributes ("padtoolbar", name, "");
		}

		internal void SetStyle (DockVisualStyle style)
		{
			//topFrame.BackgroundColor = style.PadBackgroundColor.Value.ToGdkColor ();
		}

		public void Add (Control widget)
		{
			Add (widget, false);
		}

		public void Add (Control widget, bool fill)
		{
			Add (widget, fill, -1);
		}

		public void Add (Control widget, bool fill, int padding)
		{
			Add (widget, fill, padding, -1);
		}

		void Add (Control control, bool fill, int padding, int index)
		{
			int defaultPadding = 3;

			//Widget widget = control;
			//if (widget is Button) {
			//	((Button)widget).Relief = ReliefStyle.None;
			//	((Button)widget).FocusOnClick = false;
			//	defaultPadding = 0;
			//} else if (widget is Entry) {
			//	((Entry)widget).HasFrame = false;
			//} else if (widget is ComboBox) {
			//	((ComboBox)widget).HasFrame = false;
			//} else if (widget is VSeparator)
			//	((VSeparator)widget).HeightRequest = 10;

			//if (padding == -1)
			//	padding = defaultPadding;

			//box.PackStart (widget, fill, fill, (uint)padding);
			//if (empty) {
			//	empty = false;
			//	frame.Show ();
			//}
			//if (index != -1) {
			//	Box.BoxChild bc = (Box.BoxChild)box [widget];
			//	bc.Position = index;
			//}
		}

		public void Insert (Control w, int index)
		{
			Add (w, false, 0, index);
		}

		public void Remove (Control widget)
		{
			widget.GetNativeWidget<NSView> ()
			.RemoveFromSuperview ();
		}

		public void SetAccessibleName (string v)
		{


		}

		public void SetAccessibleLabel (string v)
		{
		
		}

		public void SetAccessibleRole (string v, string v1)
		{

		}

		public void SetAccessibleDescription (string v)
		{

		}

		//TODO:
		public bool Sensitive {
			get { return true; }
			set {  }
		}

		public void ShowAll ()
		{
			//nothing
		}

		public bool Visible {
			get {
				return empty || !frame.Hidden;
			}
			set {
				frame.Hidden = !value;
			}
		}

		public Control [] Children {
			get { return box.Subviews.Select (child => (Control)child).ToArray (); }
		}
	}

	class MacDockToolbarProvider : pg.PropertyGrid.IToolbarProvider<NSView>
	{
		MacDockItemToolbar tb;
		bool visible = true;
		List<NSView> buttons = new List<NSView> ();

		public void Attach (MacDockItemToolbar tb)
		{
			if (this.tb == tb)
				return;
			this.tb = tb;
			if (tb != null) {
				tb.Visible = visible;
				foreach (var c in tb.Children)
					tb.Remove (c);
				foreach (var b in buttons)
					tb.Add (b);
			}
		}

		#region IToolbarProvider implementation

		public void Insert (NSView w, int pos)
		{
			if (tb != null)
				tb.Insert (w, pos);

			if (pos == -1)
				buttons.Add (w);
			else
				buttons.Insert (pos, w);
		}

		public void ShowAll ()
		{
			//throw new NotImplementedException ();
		}

		public NSView [] Children => buttons.ToArray ();

		public bool Visible {
			get => visible;
			set {
				visible = value;
				if (tb != null)
					tb.Visible = value;
			}
		}

		#endregion
	}

	class MacInvisibleFrame : NSView
	{
		public NSView ReplaceChild (NSView widget)
		{
			//TODO: ADD padding
			var old = Subviews.FirstOrDefault ();
			if (old != null)
				old.RemoveFromSuperview ();
			AddSubview (widget);
			return old;
		}
	}
}
