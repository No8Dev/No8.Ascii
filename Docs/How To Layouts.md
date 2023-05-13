# Flexible Layout

The built in layout system is based off [Yoga layout](https://yogalayout.com/) but with slightly different terminology and simplified options.
Direction (Left to Right, Right to Left) has been removed in favour of just using Layout Direction (Horizontal, Vertical).

## Layout Direction

Layout Direction indicated the order that elements are added to the container.

This defines the Main-Direction as Vertical (column) or Horizontal (Row).
The Cross-Direction would be the oposite.  Horizontal or Vertical

| Column | Row   | 
| :----: | :--   | 
|  1     | 1 2 3 | 
|  2     | 
|  3     | 

Based on [Yoga Flex Direction](https://yogalayout.com/docs/flex-direction/).

## Children Wrap

Do you want the child elements to wrap when the layout direction (Horz/Vert) is full

**No Wrap**
```
 [ 1 2 3 4 5 6 7 8 ]
```

**Wrap**
```
 [ 1 2 3 4 5 ]
 [ 6 7 8     ]
```

Default value is: NoWrap

Based on [Yoga Flex Wrap](https://yogalayout.com/docs/flex-wrap).


## Align Content Main

Determines how the child elements are laid out in the Main-Direction

If the Layout Direction is **Column** or **Column Reverse**, 
then the Main Direction is Horizontal, 
and the Cross Direction is Vertical

If the Layout Direction is **Row** or **Row Reverse**, 
then the Main Direction is Vertical,
and the Cross Direction is Horizontal

**Start**
```
[1 2 3          ] 
```
**End** 
```
[          1 2 3]
```

**Centre** 
```
[     1 2 3     ]
```

**Space Between** 
```
[1      2      3]
```
**Space Around** 
```
[  1    2    3  ]
```

**Space Evenly** 
```
[   1   2   3   ]
```

Default value is: **Start**

A good mental modal for this how a single line of elements are aligned and spaced

Based on [Yoga Justify Content](https://yogalayout.com/docs/justify-content).

## Align Children Cross

Determines how the child elements are laid out in the Cross-Direction.

Default value is: **Start**

A good mental modal for this how a single line of elements are aligned and spaced within that row.  
If the first item is the tallest, should the other items align to the top, botom, center...

## Align Content Cross

Determines how the `content` is aligned in the Cross-Direction, but only if there are multiple wrapped rows.

The `content` can be defined as all rows together.

## Align Override

Align Override is like an override for a single child element.
If the container has Align Children Cross set to **Start**, 
then a selection of child elements may have the Align Override set to **End**, overriding the container setting.

## Overflow

What happens to child elements when the layout direction (horz/vert) is full.  
Then can be hidden, visible or set to scroll.

This is only relevent when Children Wrap is set to NoWrap.

## Margin, Padding
Margin is surounding an element
Padding is within an element
Bounds is the size of the element (excluding margin)
ContentBounds is the size, excluding the padding.

```
┌─Container/Parent───┐
│   Margin           │
│ ╔═Bounds══════╗    │
│ ║ Padding     ║    │
│ ║ ┌─────────┐ ║    │
│ ║ │ Content │ ║    │
│ ║ │ Bounds  │ ║    │
│ ║ └─────────┘ ║    │
│ ╚═════════════╝    │
└────────────────────┘
```

## Position Type, Position, Left, Top, Right, Bottom

If the position type is relative, these are like an offset to the computed location in the canvas.

If the position type is set to absolute, then the position is set against the canvas.

## Flex, Flex Grow, Flex Shrink

**Flex Grow** describes how any space within a container should be distributed among its 
children along the main axis. After laying out its children, a container will distribute 
any remaining space according to the flex grow values specified by its children.

**Flex Shrink** describes how to shrink children along the main axis in the case that the 
total size of the children overflow the size of the container on the main axis. **Flex shrink** 
is very similar to flex grow and can be thought of in the same way if any overflowing size 
is considered to be negative remaining space. These two properties also work well together 
by allowing children to grow and shrink as needed.

**Flex** is an axis-independent way of providing the default size of an item along the main 
axis. Setting the **flex** value of a child is similar to setting the width of that child if 
its parent is a container with layout direction: **horizontal** or setting the height of a child if 
its parent is a container with layout direction: **vertical**. 
The **flex** value of an item is the default size of that item, the size of the item before 
any flex grow and flex shrink calculations are performed.

Based on [Yoga Flex](https://yogalayout.com/docs/flex).

## Width, Height

Requested Width and Height of element

## Min Width, Max Width, Min Height, Max Height

Constraints placed on the element

## Aspect Ratio

When dimentions of an element are flex, an aspect ratio can be maintained


