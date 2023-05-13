using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class ComplexLayoutTests
{
    private No8.Ascii.Canvas? _canvas;

    private void Draw( Control root, int width = 32, int height = 16 )
    {
        _canvas = new No8.Ascii.Canvas(width, height);
        ElementArrange.Calculate(root, width, height);

        root.OnDraw(_canvas, null);

        System.Console.WriteLine(root);
        System.Console.WriteLine(root.Layout);
    }

    [Fact]
    public void Test1()
    {
        var root = new Frame();

        Draw( root );

        Assert.Equal( @"┌──────────────────────────────┐
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas!.ToString() );
    }

    [Fact]
    public void Test2()
    {
        var root = new Frame
                   {
                       new LayoutPlan {
                                          Width = 16, Height = 8
                                      }
                   };

        Draw(root);

        Assert.Equal(
            @"┌──────────────┐
│              │
│              │
│              │
│              │
│              │
│              │
└──────────────┘",
            _canvas!.ToString());

        // Under some engines, the root element should always stretch to the canvas,
        // but here, we draw it as per the plan
    }

    /*
    [Fact]
    public void Test3()
    {
        var root = new TestControl
                   {
                       new TestControl
                       {
                           new LayoutPlan
                           {
                               Width  = 16,
                               Height = 8,

                               HorzPosition = LayoutPosition.Center,
                               VertPosition = LayoutPosition.Center
                           }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│                              │
│                              │
│       ┌──────────────┐       │
│       │              │       │
│       │              │       │
│       │              │       │
│       │              │       │
│       │              │       │
│       │              │       │
│       └──────────────┘       │
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test4()
    {
        var root = new Frame
                   {
                       new Frame
                       {
                           new LayoutPlan
                           {
                               Width  = 16,
                               Height = 8,

                               HorzPosition = LayoutPosition.End,
                               VertPosition = LayoutPosition.End
                           }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│              ┌──────────────┐│
│              │              ││
│              │              ││
│              │              ││
│              │              ││
│              │              ││
│              │              ││
│              └──────────────┘│
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test5()
    {
        var root = new Frame
                   {
                       new Frame { }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│┌────────────────────────────┐│
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
│└────────────────────────────┘│
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test6()
    {
        var root = new Frame
                   {
                       new Frame
                       {
                           new LayoutPlan
                           {
                               HorzPosition = LayoutPosition.Stretch,
                               VertPosition = LayoutPosition.Stretch
                           }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│┌────────────────────────────┐│
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
│└────────────────────────────┘│
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test7()
    {
        var root = new Frame
                   {
                       new Frame
                       {
                           new Frame
                           {
                               new LayoutPlan
                               {
                                   Width  = 8,
                                   Height = 4
                               }
                           }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│┌────────────────────────────┐│
││┌──────┐                    ││
│││      │                    ││
│││      │                    ││
││└──────┘                    ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
│└────────────────────────────┘│
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test8()
    {
        var root = new Frame
                   {
                       new Frame
                       {
                           new FrameStyle
                           {
                               Padding      = 1,
                               HorzPosition = LayoutPosition.Center,
                               VertPosition = LayoutPosition.Center
                           },
                           new Frame
                           {
                               new LayoutPlan
                               {
                                   Width  = 8,
                                   Height = 4
                               }
                           }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│                              │
│                              │
│         ┌──────────┐         │
│         │          │         │
│         │ ┌──────┐ │         │
│         │ │      │ │         │
│         │ │      │ │         │
│         │ └──────┘ │         │
│         │          │         │
│         └──────────┘         │
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }
        
    [Fact]
    public void Test9()
    {
        var root = new Frame
                   {
                       new Frame
                       {
                           new FrameStyle
                           {
                               HorzPosition = LayoutPosition.Center,
                               VertPosition = LayoutPosition.Center
                           },
                           new Stack
                           {
                               new StackStyle
                               {
                                   Padding = 1,
                               },
                               new Label
                               {
                                   Text         = "Label",
                                   HorzPosition = LayoutPosition.Center
                               },
                               new Button
                               {
                                   Text         = "Button",
                                   HorzPosition = LayoutPosition.Center,
                               }
                           }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│                              │
│                              │
│        ┌────────────┐        │
│        │            │        │
│        │ Label      │        │
│        │ ┌────────┐ │        │
│        │ │ Button │ │        │
│        │ ╘════════╛ │        │
│        │            │        │
│        └────────────┘        │
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }
    [Fact]
    public void Test14()
    {
        var root = new Frame
                   {
                       new Frame
                       {
                           new FrameStyle
                           {
                               MaxWidth     = 64, // width * 2
                               Height       = 8,  // height / 2
                               HorzPosition = LayoutPosition.Stretch,
                               VertPosition = LayoutPosition.Center
                           }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│                              │
│                              │
│┌────────────────────────────┐│
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
││                            ││
│└────────────────────────────┘│
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test15()
    {
        var root = new Frame
                   {
                       new Frame
                       {
                           new FrameStyle
                           {
                               Width        = 64, // width * 2
                               Height       = 8,  // height / 2
                               HorzPosition = LayoutPosition.Center,
                               VertPosition = LayoutPosition.Center
                           }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│                              │
│                              │
┼──────────────────────────────┼
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
┼──────────────────────────────┼
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test18()
    {
        var root = new Frame
                   {
                       new Label
                       {
                           Text = "Hello, World"
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│Hello, World                  │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test19()
    {
        var root = new Frame
                   {
                       new Label
                       {
                           Text         = "Hello, World",
                           HorzPosition = LayoutPosition.Center,
                           VertPosition = LayoutPosition.Center
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│         Hello, World         │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void Test20()
    {
        var root = new Frame
                   {
                       new Stack
                       {
                           new Style { VertPosition = LayoutPosition.Center },
                           new Label { Text         = "<Center text. Center text. Center text. Center text. Center text. Center text.>", HorzPosition = LayoutPosition.Center },
                           new Label { Text         = "<Right aligned. Right aligned. Right aligned. Right aligned. Right aligned>", HorzPosition   = LayoutPosition.End },
                           new Label { Text         = "<Left aligned and truncated. Left aligned and truncated>", },
                           new Label { Text         = "<Text that is much too long to fit inside the canvas so will be Wrapped>", TextWrap   = Wrap.WordWrap, },
                           new Label { Text         = "<Truncated Text that is much too long to fit inside the canvas>", },
                           new Label { Text         = "<Truncated Text with Ellipses that is much too long to fit inside the canvas>", TextWrap = Wrap.Truncate, },
                           new Label { Text         = "<Truncated Text with word break Ellipses that is much too long to fit inside the canvas>", TextWrap = Wrap.TruncateWithWord, }
                       }
                   };

        Draw(root);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│                              │
│t. Center text. Center text. C│
│ Right aligned. Right aligned>│
│<Left aligned and truncated. L│
│<Text that is much too long to│
│fit inside the canvas so will │
│be Wrapped>                   │
│<Truncated Text that is much t│
│<Truncated Text with Ellipse..│
│<Truncated Text with word..   │
│                              │
│                              │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }


    [Fact]
    public void TextTest_1()
    {
        var root = new Frame
                   {
                       new Stack
                       {
                           new Style { VertPosition = LayoutPosition.Center },
                           new Label { Text         = "<Center text. Center text. Center text. Center text. Center text. Center text.>", HorzPosition = LayoutPosition.Center },
                       }
                   };

        Draw(root, 32, 5);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│t. Center text. Center text. C│
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }

    [Fact]
    public void TextTest_2()
    {
        var root = new Frame
                   {
                       new Stack
                       {
                           new Style { VertPosition = LayoutPosition.Center },
                           new Label { Text         = "<Right aligned. Right aligned. Right aligned. Right aligned. Right aligned>", HorzPosition   = LayoutPosition.End },
                       }
                   };

        Draw(root, 32, 5);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│ Right aligned. Right aligned>│
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }
    [Fact]
    public void TextTest_3()
    {
        var root = new Frame
                   {
                       new Stack
                       {
                           new Style { VertPosition = LayoutPosition.Center },
                           new Label { Text         = "<Left aligned and truncated. Left aligned and truncated>", },
                       }
                   };

        Draw(root, 32, 5);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│<Left aligned and truncated. L│
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }
    [Fact]
    public void TextTest_4()
    {
        var root = new Frame
                   {
                       new Stack
                       {
                           new Style { VertPosition = LayoutPosition.Center },
                           new Label { Text         = "<Text that is much too long to fit inside the canvas so will be Wrapped>", TextWrap   = Wrap.WordWrap, },
                       }
                   };

        Draw(root, 32, 5);

        Assert.Equal(@"┌──────────────────────────────┐
│<Text that is much too long to│
│fit inside the canvas so will │
│be Wrapped>                   │
└──────────────────────────────┘",
            _canvas.ToString());
    }
    [Fact]
    public void TextTest_5()
    {
        var root = new Frame
                   {
                       new Stack
                       {
                           new Style { VertPosition = LayoutPosition.Center },
                           new Label { Text         = "<Truncated Text that is much too long to fit inside the canvas>", },

                       }
                   };

        Draw(root, 32, 5);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│<Truncated Text that is much t│
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }
    [Fact]
    public void TextTest_6()
    {
        var root = new Frame
                   {
                       new Stack
                       {
                           new Style { VertPosition = LayoutPosition.Center },
                           new Label { Text         = "<Truncated Text with Ellipses that is much too long to fit inside the canvas>", TextWrap = Wrap.Truncate, },
                       }
                   };

        Draw(root, 32, 5);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│<Truncated Text with Ellipse..│
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }
    [Fact]
    public void TextTest_7()
    {
        var root = new Frame
                   {
                       new Stack
                       {
                           new Style { VertPosition = LayoutPosition.Center },
                           new Label { Text         = "<Truncated Text with word break Ellipses that is much too long to fit inside the canvas>", TextWrap = Wrap.TruncateWithWord, }
                       }
                   };

        Draw(root, 32, 5);

        Assert.Equal(@"┌──────────────────────────────┐
│                              │
│<Truncated Text with word..   │
│                              │
└──────────────────────────────┘",
            _canvas.ToString());
    }
    */
}