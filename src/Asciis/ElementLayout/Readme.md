
# Arrange Elements

### step 1
**Calculate values for remainder of algorithm**

### step 2
**Determine available size in main and cross directions**

### step 3
**Determine main length for each item**

### step 4
**Collect flex items into flex lines**

### step 5
**Resolving flexible lengths on main direction**

Calculate the remaining available space that needs to be allocated. If
the main dimension size isn't known, it is computed based on the line
length, so there's no more space left to distribute.

### step 6
**Main-direction justification & cross-direction size determination**

At this point, all the elements have their dimensions set in the main
direction. Their dimensions are also set in the cross direction with the exception
of items that are aligned "stretch". We need to compute these stretch
values and set the final positions.

### step 7: 
**Cross-direction alignment**

We can skip element alignment if we're just measuring the container.

### step 8
**Multi-line content alignment**

CurrentLead stores the size of the cross dim

### step 9
**Computing final dimensions**

### step 10
**Sizing and positioning absolute elements**

