
// STEP 1: CALCULATE VALUES FOR REMAINDER OF ALGORITHM

// STEP 2: DETERMINE AVAILABLE SIZE IN MAIN AND CROSS DIRECTIONS

// STEP 3: DETERMINE MAIN LENGTH FOR EACH ITEM

// STEP 4: COLLECT FLEX ITEMS INTO FLEX LINES

// STEP 5: RESOLVING FLEXIBLE LENGTHS ON MAIN DIRECTION

// Calculate the remaining available space that needs to be allocated. If
// the main dimension size isn't known, it is computed based on the line
// length, so there's no more space left to distribute.

// STEP 6: MAIN-DIRECTION JUSTIFICATION & CROSS-DIRECTION SIZE DETERMINATION
// At this point, all the elements have their dimensions set in the main
// direction. Their dimensions are also set in the cross direction with the exception
// of items that are aligned "stretch". We need to compute these stretch
// values and set the final positions.

// STEP 7: CROSS-DIRECTION ALIGNMENT
// We can skip element alignment if we're just measuring the container.

// STEP 8: MULTI-LINE CONTENT ALIGNMENT
// currentLead stores the size of the cross dim

// STEP 9: COMPUTING FINAL DIMENSIONS

// STEP 10: SIZING AND POSITIONING ABSOLUTE ELEMENTS

