﻿#if MAC
using System;
using AppKit;
using CoreGraphics;
using Foundation;
using MonoDevelop.Ide;
using Xwt;
using System.Linq;

namespace MonoDevelop.DesignerSupport.Toolbox
{
	[Register ("LabelCollectionViewItem")]
	public class LabelCollectionViewItem : NSCollectionViewItem
	{
		public const int ItemHeight = 32;
		internal const string Name = "LabelViewItem";

		public override string Description => TextField.StringValue;

		public override bool Selected {
			get => base.Selected;
			set {
				base.Selected = value;
				if (contentCollectionView != null) {
					contentCollectionView.IsSelected = value;
				}
			}
		}

		ContentCollectionViewItem contentCollectionView;
		public override void LoadView ()
		{
			View = contentCollectionView = new ContentCollectionViewItem ();
			View.Identifier = Name;
			View.AccessibilityElement = false;
			contentCollectionView.BackgroundSelectedColor = Styles.CellBackgroundSelectedColor;
			contentCollectionView.BorderSelectedColor = Styles.CellBorderSelectedColor;
			contentCollectionView.BackgroundColor = Styles.CellBackgroundColor;

			ImageView = new NSImageView ();
			contentCollectionView.AddArrangedSubview (ImageView);
			TextField = NativeViewHelper.CreateLabel ("", NSTextAlignment.Left, NativeViewHelper.GetSystemFont (false, (int)NSFont.SmallSystemFontSize));
			contentCollectionView.AddArrangedSubview (TextField);
			contentCollectionView.EdgeInsets = new NSEdgeInsets (0, 7, 0, 0);
		}

		public LabelCollectionViewItem (IntPtr handle) : base (handle)
		{

		}
	}

	[Register ("HeaderCollectionViewItem")]
	public class HeaderCollectionViewItem : NSCollectionViewItem
	{
		public static NSImage CollapsedImage = ImageService.GetIcon ("md-disclose-arrow-down", Gtk.IconSize.Menu).ToNative ();
		public static NSImage ExpandedImage = ImageService.GetIcon ("md-disclose-arrow-up", Gtk.IconSize.Menu).ToNative ();

		public const int ExpandButtonSize = 20;
		public const int SectionHeight = 25;

		internal const string Name = "HeaderViewItem";

		bool isCollapsed;
		public bool IsCollapsed {
			get => isCollapsed;
			internal set {
				isCollapsed = value;
				ExpandButton.Image = value ? CollapsedImage : ExpandedImage;
			}
		}

		public NativeViews.ExpanderButton ExpandButton { get; private set; }

		public override void LoadView ()
		{
			View = ExpandButton = new NativeViews.ExpanderButton {
				Image = ExpandedImage,
			};
			ExpandButton.Identifier = Name;
		}

		public HeaderCollectionViewItem (IntPtr handle) : base (handle)
		{

		}
	}

	[Register ("ImageCollectionViewItem")]
	public class ImageCollectionViewItem : NSCollectionViewItem
	{
		public static CGSize Size = new CGSize (25, 25);

		internal const string Name = "ImageViewItem";
		const int margin = 5;

		public override bool Selected {
			get => base.Selected;
			set {
				base.Selected = value;
				if (contentCollectionView != null) {
					contentCollectionView.IsSelected = value;
				}
			}
		}

		public  NSImage Image {
			get => contentCollectionView.BackgroundImage;
			set {
				contentCollectionView.BackgroundImage = value;
			}
		}

		public string AccessibilityTitle {
			get => contentCollectionView.AccessibilityTitle;
			set {
				contentCollectionView.AccessibilityTitle = value;
			}
		}

		public bool AccessibilityElement {
			get => contentCollectionView.AccessibilityElement;
			set {
				contentCollectionView.AccessibilityElement = value;
			}
		}

		ContentCollectionViewItem contentCollectionView;
		public override void LoadView ()
		{
			View = contentCollectionView = new ContentCollectionViewItem ();
			contentCollectionView.Identifier = Name;
			contentCollectionView.BackgroundSelectedColor = Styles.CellBackgroundSelectedColor;
			contentCollectionView.BorderSelectedColor = Styles.CellBorderSelectedColor;
			contentCollectionView.BackgroundColor = Styles.CellBackgroundColor;
			contentCollectionView.EdgeInsets = new NSEdgeInsets (0, 0, 0, 0);
		}

		public ImageCollectionViewItem (IntPtr handle) : base (handle)
		{

		}
	}

	class ContentCollectionViewItem : NSStackView
	{
		public NSColor BackgroundColor { get; set; } = NSColor.Control;
		public NSColor BackgroundSelectedColor { get; set; } = NSColor.SelectedTextBackground;
		public NSColor BorderSelectedColor { get; internal set; }

		public NSImage BackgroundImage { get; internal set; }

		bool isSelected;
		public bool IsSelected {
			get => isSelected;
			set {
				if (isSelected == value) {
					return;
				}
				isSelected = value;
				NeedsDisplay = true;
			}
		}

		bool isMouseOver;
		public bool IsMouseOver {
			get => isMouseOver;
			set {
				if (isMouseOver == value) {
					return;
				}
				isMouseOver = value;
				NeedsDisplay = true;
			}
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			base.DrawRect (dirtyRect);

			if (IsSelected) {
				BackgroundSelectedColor.Set ();
			} else {
				BackgroundColor.Set ();
			}
			NSBezierPath.FillRect (dirtyRect);

			if (BackgroundImage != null) {
				var context = NSGraphicsContext.CurrentContext;
				context.SaveGraphicsState ();

				var point = (Frame.Width - BackgroundImage.Size.Width) / 2;
				BackgroundImage.Draw (new CGRect (point, point, BackgroundImage.Size.Width, BackgroundImage.Size.Height));
				context.RestoreGraphicsState ();
			}

			if (isMouseOver && BorderSelectedColor != null) {
				BorderSelectedColor.Set ();
				var rect = NSBezierPath.FromRect (new CGRect (dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height));
				rect.LineWidth = 1.5f;
				rect.ClosePath ();
				rect.Stroke ();
			}
		}

		public ContentCollectionViewItem ()
		{
			Orientation = NSUserInterfaceLayoutOrientation.Horizontal;
		}
	}
}
#endif