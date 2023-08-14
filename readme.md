# No8.Ascii


### Attribution

Many parts of this software are inspired and/or based on other open source code
The license from each repository is included in the LICENSE file

https://github.com/migueldeicaza/gui.cs

https://github.com/xero/figlet-fonts

https://github.com/lukesampson/figlet

https://github.com/facebook/yoga

https://github.com/brainoffline/xamarin.yoga


# Canvas

A *Canvas* object is a grid of Glyphs (Width and Height).

A *Glyph* is a Rune (ascii, utf8, unicode or emoji character) with Foreground and Background Colors.

A *Rune* (unicode) can be a single or multi character. The output width can be singular or multiple. 

U+0061 = 'a'.   // ascii lower case A

U+0250 = 'ɐ'.   // upside down lower case A

U+2200 = '∀'.   // upside down upper case A 

U+270B = '✋'.   // Hand. Single character input but when displayed, covers two spaces 

U+1F350 = '🍐'.  // Multiple character input and ouput width of 2

https://en.wikipedia.org/wiki/List_of_Unicode_characters

https://en.wikipedia.org/wiki/Emoji

With a string of characters, you can tell if a character is marked as the beginning of a unicode character.

`char.IsSurrogate`

## Drawing on the Canvas

A `Brush` can be either a solid color or a gradient.

A LineSet is used for pre-defined box drawing lines.

- SingleLine, DoubleLine, SolidLine, RoundLine, DoubleHorz, SingleHorz

```
┌─┬┐   ╔═╦╗   ┏━┳┓   ╭─┬╮  ╒═╤╕   ╓─╥╖
├─┼┤   ╠═╬╣   ┣━╋┫   ├─┼┤  ╞═╪╡   ╟─╫╢
│ ││   ║ ║║   ┃ ┃┃   │ ││  │ ││   ║ ║║
└─┴┘   ╚═╩╝   ┗━┻┛   ╰─┴╯  ╘═╧╛   ╙─╨╜
```
- Double Under, Double Over, Double Raised, Double Pressed
```
┌───────┐  ╒═══════╕  ┌───────╖  ╔═══════╕
│       │  │       │  │       ║  ║       │
│       │  │       │  │       ║  ║       │
╘═══════╛  └───────┘  ╘═══════╝  ╙───────┘
```


`Resize`: increase or decrease the Canvas size, keeping as much of the current contents as possible. 

`SetGlyph`: update a single value in the Canvas.

`WriteAt`: to update a single character or a string.

`DrawString`: Draw a string to the canvas

`DrawAlphaString`: Draw a string to the canvas, but ignore the spaces

`DrawLine`: Draw a line between two points using a LineSet.

`FillRect`: fill a solid rectangle area with a combination of Rune, Foreground and Background colors. 

`DrawRect`: Draw a box with the given LineSet

`PaintBackground`: Update a background using a brush.

`PaintForeground`: Update a foreground color using a brush.

`PaintBorderBackground`: Update just the border background using a brush.

`PaintBorderForeground`: Update just the border foreground using a brush.

`DrawTriable`

`FillTriangle`

`DrawCircle`

`FillCircle`

`DrawSprite`: Draw a pre-defined sprite

`ExportSprite`: Export the canvas as a sprite

`DrawPartialSprite`

`DrawWireframeModel`: Draw a list of Lines

`Overwrite`: Write the other Canvas over this Canvas






















