using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SizeShooter.Ui
{
    public static class UiFactory
    {
        public static VisualElement Group(FlexDirection direction = FlexDirection.Column)
        {
            return new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    flexDirection = direction
                }
            };
        }

        public static VisualElement Spacer(float size)
        {
            return new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    flexGrow = 0f,
                    height = size,
                    width = size
                }
            };
        }

        public static Label Text(string text, float size, Color color,
            FontStyle weight = FontStyle.Normal, TextAnchor align = TextAnchor.MiddleLeft)
        {
            return new Label(text)
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    fontSize = size,
                    color = color,
                    unityFontStyleAndWeight = weight,
                    unityTextAlign = align,
                    marginTop = 0,
                    marginBottom = 0,
                    marginLeft = 0,
                    marginRight = 0,
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 0,
                    paddingRight = 0
                }
            };
        }

        public static VisualElement Card(float radius = UiTheme.Radius)
        {
            VisualElement card = Group();
            card.style.backgroundColor = UiTheme.Card;
            card.style.paddingTop = 22;
            card.style.paddingBottom = 22;
            card.style.paddingLeft = 26;
            card.style.paddingRight = 26;
            SetRadius(card, radius);
            return card;
        }

        public static VisualElement Solid(Color color, float radius = 0f)
        {
            VisualElement box = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    backgroundColor = color
                }
            };

            if (radius > 0f) SetRadius(box, radius);
            return box;
        }

        public static void SetRadius(VisualElement element, float radius)
        {
            element.style.borderTopLeftRadius = radius;
            element.style.borderTopRightRadius = radius;
            element.style.borderBottomLeftRadius = radius;
            element.style.borderBottomRightRadius = radius;
        }

        public static Button Action(string text, Action onClick, float width, float height = UiTheme.ButtonHeight)
        {
            Button button = new Button(onClick)
            {
                text = text,
                pickingMode = PickingMode.Position,
                style =
                {
                    width = width,
                    height = height,
                    fontSize = 34f,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = UiTheme.Text,
                    backgroundColor = UiTheme.Button,
                    borderTopWidth = 0,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0,
                    marginTop = 0,
                    marginBottom = 0,
                    marginLeft = 0,
                    marginRight = 0,
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 0,
                    paddingRight = 0
                }
            };
            SetRadius(button, UiTheme.Radius);

            button.RegisterCallback<PointerEnterEvent>(_ => button.style.backgroundColor = UiTheme.ButtonHover);
            button.RegisterCallback<PointerLeaveEvent>(_ => button.style.backgroundColor = UiTheme.Button);
            button.RegisterCallback<PointerDownEvent>(_ => button.style.backgroundColor = UiTheme.ButtonPressed);
            button.RegisterCallback<PointerUpEvent>(_ => button.style.backgroundColor = UiTheme.ButtonHover);
            return button;
        }

        public static void FullScreen(VisualElement element)
        {
            element.style.position = Position.Absolute;
            element.style.left = 0;
            element.style.top = 0;
            element.style.right = 0;
            element.style.bottom = 0;
        }

        public static void AnchorTopLeft(VisualElement element, float left, float top)
        {
            element.style.position = Position.Absolute;
            element.style.left = left;
            element.style.top = top;
        }

        public static void AnchorTopRight(VisualElement element, float right, float top)
        {
            element.style.position = Position.Absolute;
            element.style.right = right;
            element.style.top = top;
        }
    }
}
