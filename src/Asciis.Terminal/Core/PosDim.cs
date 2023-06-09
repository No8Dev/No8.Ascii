﻿namespace Asciis.Terminal.Core
{
    /// <summary>
    /// Describes the position of a <see cref="View"/> which can be an absolute value, a percentage, centered, or 
    /// relative to the ending dimension. Integer values are implicitly convertible to
    /// an absolute <see cref="Pos"/>. These objects are created using the static methods Percent,
    /// AnchorEnd, and Center. The <see cref="Pos"/> objects can be combined with the addition and 
    /// subtraction operators.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Use the <see cref="Pos"/> objects on the X or Y properties of a view to control the position.
    ///   </para>
    ///   <para>
    ///     These can be used to set the absolute position, when merely assigning an
    ///     integer value (via the implicit integer to <see cref="Pos"/> conversion), and they can be combined
    ///     to produce more useful layouts, like: Pos.Center - 3, which would shift the position
    ///     of the <see cref="View"/> 3 characters to the left after centering for example.
    ///   </para>
    ///   <para>
    ///     It is possible to reference coordinates of another view by using the methods
    ///     Left(View), Right(View), Bottom(View), Top(View). The X(View) and Y(View) are
    ///     aliases to Left(View) and Top(View) respectively.
    ///   </para>
    /// </remarks>
    public class Pos
    {
        internal virtual int Anchor(int width) { return 0; }

        internal class PosFactor : Pos
        {
            private float factor;

            public PosFactor(float n) { factor = n; }

            internal override int Anchor(int width) { return (int)(width * factor); }

            public override string ToString() { return $"Pos.Factor({factor})"; }
        }

        /// <summary>
        /// Creates a percentage <see cref="Pos"/> object
        /// </summary>
        /// <returns>The percent <see cref="Pos"/> object.</returns>
        /// <param name="n">A value between 0 and 100 representing the percentage.</param>
        /// <example>
        /// This creates a <see cref="TextField"/>that is centered horizontally, is 50% of the way down, 
        /// is 30% the height, and is 80% the width of the <see cref="View"/> it added to.
        /// <code>
        /// var textView = new TextView () {
        ///	X = Pos.Center (),
        ///	Y = Pos.Percent (50),
        ///	Width = Dim.Percent (80),
        /// 	Height = Dim.Percent (30),
        /// };
        /// </code>
        /// </example>
        public static Pos Percent(float n)
        {
            if (n < 0 || n > 100)
                throw new ArgumentException("Percent value must be between 0 and 100");

            return new PosFactor(n / 100);
        }

        private static PosAnchorEnd endNoMargin;

        internal class PosAnchorEnd : Pos
        {
            private int n;

            public PosAnchorEnd(int n) { this.n = n; }

            internal override int Anchor(int width) { return width - n; }

            public override string ToString() { return $"Pos.AnchorEnd(margin={n})"; }
        }

        /// <summary>
        /// Creates a <see cref="Pos"/> object that is anchored to the end (right side or bottom) of the dimension, 
        /// useful to flush the layout from the right or bottom.
        /// </summary>
        /// <returns>The <see cref="Pos"/> object anchored to the end (the bottom or the right side).</returns>
        /// <param name="margin">Optional margin to place to the right or below.</param>
        /// <example>
        /// This sample shows how align a <see cref="Button"/> to the bottom-right of a <see cref="View"/>.
        /// <code>
        /// // See Issue #502 
        /// anchorButton.X = Pos.AnchorEnd () - (Pos.Right (anchorButton) - Pos.Left (anchorButton));
        /// anchorButton.Y = Pos.AnchorEnd (1);
        /// </code>
        /// </example>
        public static Pos AnchorEnd(int margin = 0)
        {
            if (margin < 0)
                throw new ArgumentException("Margin must be positive");

            if (margin == 0)
            {
                if (endNoMargin == null)
                    endNoMargin = new PosAnchorEnd(0);
                return endNoMargin;
            }

            return new PosAnchorEnd(margin);
        }

        internal class PosCenter : Pos
        {
            internal override int Anchor(int width) { return width / 2; }

            public override string ToString() { return "Pos.Center"; }
        }

        private static PosCenter pCenter;

        /// <summary>
        /// Returns a <see cref="Pos"/> object that can be used to center the <see cref="View"/>
        /// </summary>
        /// <returns>The center Pos.</returns>
        /// <example>
        /// This creates a <see cref="TextField"/>that is centered horizontally, is 50% of the way down, 
        /// is 30% the height, and is 80% the width of the <see cref="View"/> it added to.
        /// <code>
        /// var textView = new TextView () {
        ///	X = Pos.Center (),
        ///	Y = Pos.Percent (50),
        ///	Width = Dim.Percent (80),
        /// 	Height = Dim.Percent (30),
        /// };
        /// </code>
        /// </example>
        public static Pos Center()
        {
            if (pCenter == null)
                pCenter = new PosCenter();
            return pCenter;
        }

        internal class PosAbsolute : Pos
        {
            private int n;
            public PosAbsolute(int n) { this.n = n; }

            public override string ToString() { return $"Pos.Absolute({n})"; }

            internal override int Anchor(int width) { return n; }

            public override int GetHashCode() { return n.GetHashCode(); }

            public override bool Equals(object other) { return other is PosAbsolute abs && abs.n == n; }
        }

        /// <summary>
        /// Creates an Absolute <see cref="Pos"/> from the specified integer value.
        /// </summary>
        /// <returns>The Absolute <see cref="Pos"/>.</returns>
        /// <param name="n">The value to convert to the <see cref="Pos"/> .</param>
        public static implicit operator Pos(int n) { return new PosAbsolute(n); }

        /// <summary>
        /// Creates an Absolute <see cref="Pos"/> from the specified integer value.
        /// </summary>
        /// <returns>The Absolute <see cref="Pos"/>.</returns>
        /// <param name="n">The value to convert to the <see cref="Pos"/>.</param>
        public static Pos At(int n) { return new PosAbsolute(n); }

        internal class PosCombine : Pos
        {
            internal Pos left, right;
            private bool add;

            public PosCombine(bool add, Pos left, Pos right)
            {
                this.left = left;
                this.right = right;
                this.add = add;
            }

            internal override int Anchor(int width)
            {
                var la = left.Anchor(width);
                var ra = right.Anchor(width);
                if (add)
                    return la + ra;
                else
                    return la - ra;
            }

            public override string ToString()
            {
                return $"Pos.Combine({left.ToString()}{(add ? '+' : '-')}{right.ToString()})";
            }
        }

        private static PosCombine posCombine;

        /// <summary>
        /// Adds a <see cref="Pos"/> to a <see cref="Pos"/>, yielding a new <see cref="Pos"/>.
        /// </summary>
        /// <param name="left">The first <see cref="Pos"/> to add.</param>
        /// <param name="right">The second <see cref="Pos"/> to add.</param>
        /// <returns>The <see cref="Pos"/> that is the sum of the values of <c>left</c> and <c>right</c>.</returns>
        public static Pos operator +(Pos left, Pos right)
        {
            if (left is PosAbsolute && right is PosAbsolute)
            {
                posCombine = null;
                return new PosAbsolute(left.Anchor(0) + right.Anchor(0));
            }

            PosCombine newPos = new(true, left, right);
            SetPosCombine(left, newPos);
            return posCombine = newPos;
        }

        /// <summary>
        /// Subtracts a <see cref="Pos"/> from a <see cref="Pos"/>, yielding a new <see cref="Pos"/>.
        /// </summary>
        /// <param name="left">The <see cref="Pos"/> to subtract from (the minuend).</param>
        /// <param name="right">The <see cref="Pos"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="Pos"/> that is the <c>left</c> minus <c>right</c>.</returns>
        public static Pos operator -(Pos left, Pos right)
        {
            if (left is PosAbsolute && right is PosAbsolute)
            {
                posCombine = null;
                return new PosAbsolute(left.Anchor(0) - right.Anchor(0));
            }

            PosCombine newPos = new(false, left, right);
            SetPosCombine(left, newPos);
            return posCombine = newPos;
        }

        private static void SetPosCombine(Pos left, PosCombine newPos)
        {
            if (posCombine?.ToString() != newPos.ToString())
            {
                var view = left as PosView;
                if (view != null) view.Target.SetNeedsLayout();
            }
        }

        internal class PosView : Pos
        {
            public View Target;
            private int side;

            public PosView(View view, int side)
            {
                Target = view;
                this.side = side;
            }

            internal override int Anchor(int width)
            {
                switch (side)
                {
                    case 0: return Target.Frame.X;
                    case 1: return Target.Frame.Y;
                    case 2: return Target.Frame.Right;
                    case 3: return Target.Frame.Bottom;
                    default:
                        return 0;
                }
            }

            public override string ToString()
            {
                string tside;
                switch (side)
                {
                    case 0:
                        tside = "x";
                        break;
                    case 1:
                        tside = "y";
                        break;
                    case 2:
                        tside = "right";
                        break;
                    case 3:
                        tside = "bottom";
                        break;
                    default:
                        tside = "unknown";
                        break;
                }

                return $"Pos.View(side={tside}, target={Target.ToString()})";
            }
        }

        /// <summary>
        /// Returns a <see cref="Pos"/> object tracks the Left (X) position of the specified <see cref="View"/>.
        /// </summary>
        /// <returns>The <see cref="Pos"/> that depends on the other view.</returns>
        /// <param name="view">The <see cref="View"/>  that will be tracked.</param>
        public static Pos Left(View view) { return new PosCombine(true, new PosView(view, 0), new PosAbsolute(0)); }

        /// <summary>
        /// Returns a <see cref="Pos"/> object tracks the Left (X) position of the specified <see cref="View"/>.
        /// </summary>
        /// <returns>The <see cref="Pos"/> that depends on the other view.</returns>
        /// <param name="view">The <see cref="View"/>  that will be tracked.</param>
        public static Pos X(View view) { return new PosCombine(true, new PosView(view, 0), new PosAbsolute(0)); }

        /// <summary>
        /// Returns a <see cref="Pos"/> object tracks the Top (Y) position of the specified <see cref="View"/>.
        /// </summary>
        /// <returns>The <see cref="Pos"/> that depends on the other view.</returns>
        /// <param name="view">The <see cref="View"/>  that will be tracked.</param>
        public static Pos Top(View view) { return new PosCombine(true, new PosView(view, 1), new PosAbsolute(0)); }

        /// <summary>
        /// Returns a <see cref="Pos"/> object tracks the Top (Y) position of the specified <see cref="View"/>.
        /// </summary>
        /// <returns>The <see cref="Pos"/> that depends on the other view.</returns>
        /// <param name="view">The <see cref="View"/>  that will be tracked.</param>
        public static Pos Y(View view) { return new PosCombine(true, new PosView(view, 1), new PosAbsolute(0)); }

        /// <summary>
        /// Returns a <see cref="Pos"/> object tracks the Right (X+Width) coordinate of the specified <see cref="View"/>.
        /// </summary>
        /// <returns>The <see cref="Pos"/> that depends on the other view.</returns>
        /// <param name="view">The <see cref="View"/>  that will be tracked.</param>
        public static Pos Right(View view) { return new PosCombine(true, new PosView(view, 2), new PosAbsolute(0)); }

        /// <summary>
        /// Returns a <see cref="Pos"/> object tracks the Bottom (Y+Height) coordinate of the specified <see cref="View"/> 
        /// </summary>
        /// <returns>The <see cref="Pos"/> that depends on the other view.</returns>
        /// <param name="view">The <see cref="View"/>  that will be tracked.</param>
        public static Pos Bottom(View view) { return new PosCombine(true, new PosView(view, 3), new PosAbsolute(0)); }
    }

    /// <summary>
    /// Dim properties of a <see cref="View"/> to control the position.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Use the Dim objects on the Width or Height properties of a <see cref="View"/> to control the position.
    ///   </para>
    ///   <para>
    ///     These can be used to set the absolute position, when merely assigning an
    ///     integer value (via the implicit integer to Pos conversion), and they can be combined
    ///     to produce more useful layouts, like: Pos.Center - 3, which would shift the position
    ///     of the <see cref="View"/> 3 characters to the left after centering for example.
    ///   </para>
    /// </remarks>
    public class Dim
    {
        internal virtual int Anchor(int width) { return 0; }

        internal class DimFactor : Dim
        {
            private float factor;
            private bool remaining;

            public DimFactor(float n, bool r = false)
            {
                factor = n;
                remaining = r;
            }

            internal override int Anchor(int width) { return (int)(width * factor); }

            public bool IsFromRemaining() { return remaining; }

            public override string ToString() { return $"Dim.Factor(factor={factor}, remaining={remaining})"; }

            public override int GetHashCode() { return factor.GetHashCode(); }

            public override bool Equals(object other)
            {
                return other is DimFactor f && f.factor == factor && f.remaining == remaining;
            }
        }

        /// <summary>
        /// Creates a percentage <see cref="Dim"/> object
        /// </summary>
        /// <returns>The percent <see cref="Dim"/> object.</returns>
        /// <param name="n">A value between 0 and 100 representing the percentage.</param>
        /// <param name="r">If <c>true</c> the Percent is computed based on the remaining space after the X/Y anchor positions. If <c>false</c> is computed based on the whole original space.</param>
        /// <example>
        /// This initializes a <see cref="TextField"/>that is centered horizontally, is 50% of the way down, 
        /// is 30% the height, and is 80% the width of the <see cref="View"/> it added to.
        /// <code>
        /// var textView = new TextView () {
        ///	X = Pos.Center (),
        ///	Y = Pos.Percent (50),
        ///	Width = Dim.Percent (80),
        /// 	Height = Dim.Percent (30),
        /// };
        /// </code>
        /// </example>
        public static Dim Percent(float n, bool r = false)
        {
            if (n < 0 || n > 100)
                throw new ArgumentException("Percent value must be between 0 and 100");

            return new DimFactor(n / 100, r);
        }

        internal class DimAbsolute : Dim
        {
            private int n;
            public DimAbsolute(int n) { this.n = n; }

            public override string ToString() { return $"Dim.Absolute({n})"; }

            internal override int Anchor(int width) { return n; }

            public override int GetHashCode() { return n.GetHashCode(); }

            public override bool Equals(object other) { return other is DimAbsolute abs && abs.n == n; }
        }

        internal class DimFill : Dim
        {
            private int margin;
            public DimFill(int margin) { this.margin = margin; }

            public override string ToString() { return $"Dim.Fill(margin={margin})"; }

            internal override int Anchor(int width) { return width - margin; }

            public override int GetHashCode() { return margin.GetHashCode(); }

            public override bool Equals(object other) { return other is DimFill fill && fill.margin == margin; }
        }

        private static DimFill zeroMargin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dim"/> class that fills the dimension, but leaves the specified number of colums for a margin.
        /// </summary>
        /// <returns>The Fill dimension.</returns>
        /// <param name="margin">Margin to use.</param>
        public static Dim Fill(int margin = 0)
        {
            if (margin == 0)
            {
                if (zeroMargin == null)
                    zeroMargin = new DimFill(0);
                return zeroMargin;
            }

            return new DimFill(margin);
        }

        /// <summary>
        /// Creates an Absolute <see cref="Dim"/> from the specified integer value.
        /// </summary>
        /// <returns>The Absolute <see cref="Dim"/>.</returns>
        /// <param name="n">The value to convert to the pos.</param>
        public static implicit operator Dim(int n) { return new DimAbsolute(n); }

        /// <summary>
        /// Creates an Absolute <see cref="Dim"/> from the specified integer value.
        /// </summary>
        /// <returns>The Absolute <see cref="Dim"/>.</returns>
        /// <param name="n">The value to convert to the <see cref="Dim"/>.</param>
        public static Dim Sized(int n) { return new DimAbsolute(n); }

        internal class DimCombine : Dim
        {
            internal Dim left, right;
            private bool add;

            public DimCombine(bool add, Dim left, Dim right)
            {
                this.left = left;
                this.right = right;
                this.add = add;
            }

            internal override int Anchor(int width)
            {
                var la = left.Anchor(width);
                var ra = right.Anchor(width);
                if (add)
                    return la + ra;
                else
                    return la - ra;
            }

            public override string ToString()
            {
                return $"Dim.Combine({left.ToString()}{(add ? '+' : '-')}{right.ToString()})";
            }
        }

        private static DimCombine dimCombine;

        /// <summary>
        /// Adds a <see cref="Dim"/> to a <see cref="Dim"/>, yielding a new <see cref="Dim"/>.
        /// </summary>
        /// <param name="left">The first <see cref="Dim"/> to add.</param>
        /// <param name="right">The second <see cref="Dim"/> to add.</param>
        /// <returns>The <see cref="Dim"/> that is the sum of the values of <c>left</c> and <c>right</c>.</returns>
        public static Dim operator +(Dim left, Dim right)
        {
            if (left is DimAbsolute && right is DimAbsolute)
            {
                dimCombine = null;
                return new DimAbsolute(left.Anchor(0) + right.Anchor(0));
            }

            DimCombine newDim = new(true, left, right);
            SetDimCombine(left, newDim);
            return dimCombine = newDim;
        }

        /// <summary>
        /// Subtracts a <see cref="Dim"/> from a <see cref="Dim"/>, yielding a new <see cref="Dim"/>.
        /// </summary>
        /// <param name="left">The <see cref="Dim"/> to subtract from (the minuend).</param>
        /// <param name="right">The <see cref="Dim"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="Dim"/> that is the <c>left</c> minus <c>right</c>.</returns>
        public static Dim operator -(Dim left, Dim right)
        {
            if (left is DimAbsolute && right is DimAbsolute)
            {
                dimCombine = null;
                return new DimAbsolute(left.Anchor(0) - right.Anchor(0));
            }

            DimCombine newDim = new(false, left, right);
            SetDimCombine(left, newDim);
            return dimCombine = newDim;
        }

        private static void SetDimCombine(Dim left, DimCombine newPos)
        {
            if (dimCombine?.ToString() != newPos.ToString())
            {
                var view = left as DimView;
                if (view != null) view.Target.SetNeedsLayout();
            }
        }

        internal class DimView : Dim
        {
            public View Target;
            private int side;

            public DimView(View view, int side)
            {
                Target = view;
                this.side = side;
            }

            public override string ToString()
            {
                string tside;
                switch (side)
                {
                    case 0:
                        tside = "Height";
                        break;
                    case 1:
                        tside = "Width";
                        break;
                    default:
                        tside = "unknown";
                        break;
                }

                return $"DimView(side={tside}, target={Target})";
            }

            internal override int Anchor(int width)
            {
                switch (side)
                {
                    case 0: return Target.Frame.Height;
                    case 1: return Target.Frame.Width;
                    default:
                        return 0;
                }
            }

            public override int GetHashCode() { return Target.GetHashCode(); }

            public override bool Equals(object other) { return other is DimView abs && abs.Target == Target; }
        }

        /// <summary>
        /// Returns a <see cref="Dim"/> object tracks the Width of the specified <see cref="View"/>.
        /// </summary>
        /// <returns>The <see cref="Dim"/> of the other <see cref="View"/>.</returns>
        /// <param name="view">The view that will be tracked.</param>
        public static Dim Width(View view) { return new DimView(view, 1); }

        /// <summary>
        /// Returns a <see cref="Dim"/> object tracks the Height of the specified <see cref="View"/>.
        /// </summary>
        /// <returns>The <see cref="Dim"/> of the other <see cref="View"/>.</returns>
        /// <param name="view">The view that will be tracked.</param>
        public static Dim Height(View view) { return new DimView(view, 0); }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() { return GetHashCode(); }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="other">The object to compare with the current object. </param>
        /// <returns>
        ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object other) { return other is Dim abs && abs == this; }
    }
}