﻿using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;
using Xunit.Abstractions;

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class ComputedMarginTest : BaseTests
{
    public ComputedMarginTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void computed_layout_margin()
    {
        var root = new TestNode(new LayoutPlan { Width = 29, Height = 16, Margin = new Sides(start: 10.Percent()) });

        ElementArrange.Calculate(root, 32, 16);

        Draw(root);
        Assert.Equal(
            @"   ╔═══════════════════════════╗
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
   ╚═══════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(3, 0, 29, 16), root.Layout.Bounds);
    }
}
