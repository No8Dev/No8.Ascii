namespace No8.Ascii.TermInfo;

public sealed partial class TermInfoDesc
{
    /// <summary>
    ///     auto_left_margin [auto_left_margin, bw]: cub 1 wraps from column 0 to last column.
    /// </summary>
    public bool? AutoLeftMargin => GetBoolean(TermInfoCaps.Boolean.AutoLeftMargin);

    /// <summary>
    ///     auto_right_margin [auto_right_margin, am]: terminal has automatic margins.
    /// </summary>
    public bool? AutoRightMargin => GetBoolean(TermInfoCaps.Boolean.AutoRightMargin);

    /// <summary>
    ///     no_esc_ctlc [no_esc_ctlc, xsb]: beehive f 1 escape f 2 ctrl c.
    /// </summary>
    public bool? NoEscCtlc => GetBoolean(TermInfoCaps.Boolean.NoEscCtlc);

    /// <summary>
    ///     ceol_standout_glitch [ceol_standout_glitch, xhp]: standout not erased by overwriting hp.
    /// </summary>
    public bool? CeolStandoutGlitch => GetBoolean(TermInfoCaps.Boolean.CeolStandoutGlitch);

    /// <summary>
    ///     eat_newline_glitch [eat_newline_glitch, xenl]: newline ignored after 80 cols concept.
    /// </summary>
    public bool? EatNewlineGlitch => GetBoolean(TermInfoCaps.Boolean.EatNewlineGlitch);

    /// <summary>
    ///     erase_overstrike [erase_overstrike, eo]: can erase overstrikes with a blank.
    /// </summary>
    public bool? EraseOverstrike => GetBoolean(TermInfoCaps.Boolean.EraseOverstrike);

    /// <summary>
    ///     generic_type [generic_type, gn]: generic line type.
    /// </summary>
    public bool? GenericType => GetBoolean(TermInfoCaps.Boolean.GenericType);

    /// <summary>
    ///     hard_copy [hard_copy, hc]: hardcopy terminal.
    /// </summary>
    public bool? HardCopy => GetBoolean(TermInfoCaps.Boolean.HardCopy);

    /// <summary>
    ///     has_meta_key [has_meta_key, km]: has a meta key (i.e., sets 8th bit).
    /// </summary>
    public bool? HasMetaKey => GetBoolean(TermInfoCaps.Boolean.HasMetaKey);

    /// <summary>
    ///     has_status_line [has_status_line, hs]: has extra status line.
    /// </summary>
    public bool? HasStatusLine => GetBoolean(TermInfoCaps.Boolean.HasStatusLine);

    /// <summary>
    ///     insert_null_glitch [insert_null_glitch, in]: insert mode distinguishes nulls.
    /// </summary>
    public bool? InsertNullGlitch => GetBoolean(TermInfoCaps.Boolean.InsertNullGlitch);

    /// <summary>
    ///     memory_above [memory_above, da]: display may be retained above the screen.
    /// </summary>
    public bool? MemoryAbove => GetBoolean(TermInfoCaps.Boolean.MemoryAbove);

    /// <summary>
    ///     memory_below [memory_below, db]: display may be retained below the screen.
    /// </summary>
    public bool? MemoryBelow => GetBoolean(TermInfoCaps.Boolean.MemoryBelow);

    /// <summary>
    ///     move_insert_mode [move_insert_mode, mir]: safe to move while in insert mode.
    /// </summary>
    public bool? MoveInsertMode => GetBoolean(TermInfoCaps.Boolean.MoveInsertMode);

    /// <summary>
    ///     move_standout_mode [move_standout_mode, msgr]: safe to move while in standout mode.
    /// </summary>
    public bool? MoveStandoutMode => GetBoolean(TermInfoCaps.Boolean.MoveStandoutMode);

    /// <summary>
    ///     over_strike [over_strike, os]: terminal can overstrike.
    /// </summary>
    public bool? OverStrike => GetBoolean(TermInfoCaps.Boolean.OverStrike);

    /// <summary>
    ///     status_line_esc_ok [status_line_esc_ok, eslok]: escape can be used on the status line.
    /// </summary>
    public bool? StatusLineEscOk => GetBoolean(TermInfoCaps.Boolean.StatusLineEscOk);

    /// <summary>
    ///     dest_tabs_magic_smso [dest_tabs_magic_smso, xt]: tabs destructive magic so char t 1061.
    /// </summary>
    public bool? DestTabsMagicSmso => GetBoolean(TermInfoCaps.Boolean.DestTabsMagicSmso);

    /// <summary>
    ///     tilde_glitch [tilde_glitch, hz]: cannot print s hazeltine.
    /// </summary>
    public bool? TildeGlitch => GetBoolean(TermInfoCaps.Boolean.TildeGlitch);

    /// <summary>
    ///     transparent_underline [transparent_underline, ul]: underline character overstrikes.
    /// </summary>
    public bool? TransparentUnderline => GetBoolean(TermInfoCaps.Boolean.TransparentUnderline);

    /// <summary>
    ///     xon_xoff [xon_xoff, xon]: terminal uses xon xoff handshaking.
    /// </summary>
    public bool? XonXoff => GetBoolean(TermInfoCaps.Boolean.XonXoff);

    /// <summary>
    ///     needs_xon_xoff [needs_xon_xoff, nxon]: padding will not work xon xoff required.
    /// </summary>
    public bool? NeedsXonXoff => GetBoolean(TermInfoCaps.Boolean.NeedsXonXoff);

    /// <summary>
    ///     prtr_silent [prtr_silent, mc5i]: printer will not echo on screen.
    /// </summary>
    public bool? PrtrSilent => GetBoolean(TermInfoCaps.Boolean.PrtrSilent);

    /// <summary>
    ///     hard_cursor [hard_cursor, chts]: cursor is hard to see.
    /// </summary>
    public bool? HardCursor => GetBoolean(TermInfoCaps.Boolean.HardCursor);

    /// <summary>
    ///     non_rev_rmcup [non_rev_rmcup, nrrmc]: smcup does not reverse rmcup.
    /// </summary>
    public bool? NonRevRmcup => GetBoolean(TermInfoCaps.Boolean.NonRevRmcup);

    /// <summary>
    ///     no_pad_char [no_pad_char, npc]: pad character does not exist.
    /// </summary>
    public bool? NoPadChar => GetBoolean(TermInfoCaps.Boolean.NoPadChar);

    /// <summary>
    ///     non_dest_scroll_region [non_dest_scroll_region, ndscr]: scrolling region is non destructive.
    /// </summary>
    public bool? NonDestScrollRegion => GetBoolean(TermInfoCaps.Boolean.NonDestScrollRegion);

    /// <summary>
    ///     can_change [can_change, ccc]: terminal can re define existing colors.
    /// </summary>
    public bool? CanChange => GetBoolean(TermInfoCaps.Boolean.CanChange);

    /// <summary>
    ///     back_color_erase [back_color_erase, bce]: screen erased with background color.
    /// </summary>
    public bool? BackColorErase => GetBoolean(TermInfoCaps.Boolean.BackColorErase);

    /// <summary>
    ///     hue_lightness_saturation [hue_lightness_saturation, hls]: terminal uses only hls color notation tektronix.
    /// </summary>
    public bool? HueLightnessSaturation => GetBoolean(TermInfoCaps.Boolean.HueLightnessSaturation);

    /// <summary>
    ///     col_addr_glitch [col_addr_glitch, xhpa]: only positive motion for hpa mhpa caps.
    /// </summary>
    public bool? ColAddrGlitch => GetBoolean(TermInfoCaps.Boolean.ColAddrGlitch);

    /// <summary>
    ///     cr_cancels_micro_mode [cr_cancels_micro_mode, crxm]: using cr turns off micro mode.
    /// </summary>
    public bool? CrCancelsMicroMode => GetBoolean(TermInfoCaps.Boolean.CrCancelsMicroMode);

    /// <summary>
    ///     has_print_wheel [has_print_wheel, daisy]: printer needs operator to change character set.
    /// </summary>
    public bool? HasPrintWheel => GetBoolean(TermInfoCaps.Boolean.HasPrintWheel);

    /// <summary>
    ///     row_addr_glitch [row_addr_glitch, xvpa]: only positive motion for vpa mvpa caps.
    /// </summary>
    public bool? RowAddrGlitch => GetBoolean(TermInfoCaps.Boolean.RowAddrGlitch);

    /// <summary>
    ///     semi_auto_right_margin [semi_auto_right_margin, sam]: printing in last column causes cr.
    /// </summary>
    public bool? SemiAutoRightMargin => GetBoolean(TermInfoCaps.Boolean.SemiAutoRightMargin);

    /// <summary>
    ///     cpi_changes_res [cpi_changes_res, cpix]: changing character pitch changes resolution.
    /// </summary>
    public bool? CpiChangesRes => GetBoolean(TermInfoCaps.Boolean.CpiChangesRes);

    /// <summary>
    ///     lpi_changes_res [lpi_changes_res, lpix]: changing line pitch changes resolution.
    /// </summary>
    public bool? LpiChangesRes => GetBoolean(TermInfoCaps.Boolean.LpiChangesRes);

    /// <summary>
    ///     backspaces_with_bs [backspaces_with_bs, OTbs]: uses h to move left.
    /// </summary>
    public bool? BackspacesWithBs => GetBoolean(TermInfoCaps.Boolean.BackspacesWithBs);

    /// <summary>
    ///     crt_no_scrolling [crt_no_scrolling, OTns]: crt cannot scroll.
    /// </summary>
    public bool? CrtNoScrolling => GetBoolean(TermInfoCaps.Boolean.CrtNoScrolling);

    /// <summary>
    ///     no_correctly_working_cr [no_correctly_working_cr, OTnc]: no way to go to start of line.
    /// </summary>
    public bool? NoCorrectlyWorkingCr => GetBoolean(TermInfoCaps.Boolean.NoCorrectlyWorkingCr);

    /// <summary>
    ///     gnu_has_meta_key [gnu_has_meta_key, OTMT]: has meta key.
    /// </summary>
    public bool? GnuHasMetaKey => GetBoolean(TermInfoCaps.Boolean.GnuHasMetaKey);

    /// <summary>
    ///     linefeed_is_newline [linefeed_is_newline, OTNL]: move down with n.
    /// </summary>
    public bool? LinefeedIsNewline => GetBoolean(TermInfoCaps.Boolean.LinefeedIsNewline);

    /// <summary>
    ///     has_hardware_tabs [has_hardware_tabs, OTpt]: has 8 char tabs invoked with ^i.
    /// </summary>
    public bool? HasHardwareTabs => GetBoolean(TermInfoCaps.Boolean.HasHardwareTabs);

    /// <summary>
    ///     return_does_clr_eol [return_does_clr_eol, OTxr]: return clears the line.
    /// </summary>
    public bool? ReturnDoesClrEol => GetBoolean(TermInfoCaps.Boolean.ReturnDoesClrEol);

    /// <summary>
    ///     columns [columns, cols]: number of columns in a line.
    /// </summary>
    public int? Columns => GetNum(TermInfoCaps.Num.Columns);

    /// <summary>
    ///     init_tabs [init_tabs, it]: tabs initially every spaces.
    /// </summary>
    public int? InitTabs => GetNum(TermInfoCaps.Num.InitTabs);

    /// <summary>
    ///     lines [lines, lines]: number of lines on screen or page.
    /// </summary>
    public int? Lines => GetNum(TermInfoCaps.Num.Lines);

    /// <summary>
    ///     lines_of_memory [lines_of_memory, lm]: lines of memory if line 0 means varies.
    /// </summary>
    public int? LinesOfMemory => GetNum(TermInfoCaps.Num.LinesOfMemory);

    /// <summary>
    ///     magic_cookie_glitch [magic_cookie_glitch, xmc]: number of blank characters left by smso or rmso.
    /// </summary>
    public int? MagicCookieGlitch => GetNum(TermInfoCaps.Num.MagicCookieGlitch);

    /// <summary>
    ///     padding_baud_rate [padding_baud_rate, pb]: lowest baud rate where padding needed.
    /// </summary>
    public int? PaddingBaudRate => GetNum(TermInfoCaps.Num.PaddingBaudRate);

    /// <summary>
    ///     virtual_terminal [virtual_terminal, vt]: virtual terminal number cb unix.
    /// </summary>
    public int? VirtualTerminal => GetNum(TermInfoCaps.Num.VirtualTerminal);

    /// <summary>
    ///     width_status_line [width_status_line, wsl]: number of columns in status line.
    /// </summary>
    public int? WidthStatusLine => GetNum(TermInfoCaps.Num.WidthStatusLine);

    /// <summary>
    ///     num_labels [num_labels, nlab]: number of labels on screen.
    /// </summary>
    public int? NumLabels => GetNum(TermInfoCaps.Num.NumLabels);

    /// <summary>
    ///     label_height [label_height, lh]: rows in each label.
    /// </summary>
    public int? LabelHeight => GetNum(TermInfoCaps.Num.LabelHeight);

    /// <summary>
    ///     label_width [label_width, lw]: columns in each label.
    /// </summary>
    public int? LabelWidth => GetNum(TermInfoCaps.Num.LabelWidth);

    /// <summary>
    ///     max_attributes [max_attributes, ma]: maximum combined attributes terminal can handle.
    /// </summary>
    public int? MaxAttributes => GetNum(TermInfoCaps.Num.MaxAttributes);

    /// <summary>
    ///     maximum_windows [maximum_windows, wnum]: maximum number of definable windows.
    /// </summary>
    public int? MaximumWindows => GetNum(TermInfoCaps.Num.MaximumWindows);

    /// <summary>
    ///     max_colors [max_colors, colors]: maximum number of colors on screen.
    /// </summary>
    public int? MaxColors => GetNum(TermInfoCaps.Num.MaxColors);

    /// <summary>
    ///     max_pairs [max_pairs, pairs]: maximum number of color pairs on the screen.
    /// </summary>
    public int? MaxPairs => GetNum(TermInfoCaps.Num.MaxPairs);

    /// <summary>
    ///     no_color_video [no_color_video, ncv]: video attributes that cannot be used with colors.
    /// </summary>
    public int? NoColorVideo => GetNum(TermInfoCaps.Num.NoColorVideo);

    /// <summary>
    ///     buffer_capacity [buffer_capacity, bufsz]: numbers of bytes buffered before printing.
    /// </summary>
    public int? BufferCapacity => GetNum(TermInfoCaps.Num.BufferCapacity);

    /// <summary>
    ///     dot_vert_spacing [dot_vert_spacing, spinv]: spacing of pins vertically in pins per inch.
    /// </summary>
    public int? DotVertSpacing => GetNum(TermInfoCaps.Num.DotVertSpacing);

    /// <summary>
    ///     dot_horz_spacing [dot_horz_spacing, spinh]: spacing of dots horizontally in dots per inch.
    /// </summary>
    public int? DotHorzSpacing => GetNum(TermInfoCaps.Num.DotHorzSpacing);

    /// <summary>
    ///     max_micro_address [max_micro_address, maddr]: maximum value in micro ... address.
    /// </summary>
    public int? MaxMicroAddress => GetNum(TermInfoCaps.Num.MaxMicroAddress);

    /// <summary>
    ///     max_micro_jump [max_micro_jump, mjump]: maximum value in parm ... micro.
    /// </summary>
    public int? MaxMicroJump => GetNum(TermInfoCaps.Num.MaxMicroJump);

    /// <summary>
    ///     micro_col_size [micro_col_size, mcs]: character step size when in micro mode.
    /// </summary>
    public int? MicroColSize => GetNum(TermInfoCaps.Num.MicroColSize);

    /// <summary>
    ///     micro_line_size [micro_line_size, mls]: line step size when in micro mode.
    /// </summary>
    public int? MicroLineSize => GetNum(TermInfoCaps.Num.MicroLineSize);

    /// <summary>
    ///     number_of_pins [number_of_pins, npins]: numbers of pins in print head.
    /// </summary>
    public int? NumberOfPins => GetNum(TermInfoCaps.Num.NumberOfPins);

    /// <summary>
    ///     output_res_char [output_res_char, orc]: horizontal resolution in units per line.
    /// </summary>
    public int? OutputResChar => GetNum(TermInfoCaps.Num.OutputResChar);

    /// <summary>
    ///     output_res_line [output_res_line, orl]: vertical resolution in units per line.
    /// </summary>
    public int? OutputResLine => GetNum(TermInfoCaps.Num.OutputResLine);

    /// <summary>
    ///     output_res_horz_inch [output_res_horz_inch, orhi]: horizontal resolution in units per inch.
    /// </summary>
    public int? OutputResHorzInch => GetNum(TermInfoCaps.Num.OutputResHorzInch);

    /// <summary>
    ///     output_res_vert_inch [output_res_vert_inch, orvi]: vertical resolution in units per inch.
    /// </summary>
    public int? OutputResVertInch => GetNum(TermInfoCaps.Num.OutputResVertInch);

    /// <summary>
    ///     print_rate [print_rate, cps]: print rate in characters per second.
    /// </summary>
    public int? PrintRate => GetNum(TermInfoCaps.Num.PrintRate);

    /// <summary>
    ///     wide_char_size [wide_char_size, widcs]: character step size when in double wide mode.
    /// </summary>
    public int? WideCharSize => GetNum(TermInfoCaps.Num.WideCharSize);

    /// <summary>
    ///     buttons [buttons, btns]: number of buttons on mouse.
    /// </summary>
    public int? Buttons => GetNum(TermInfoCaps.Num.Buttons);

    /// <summary>
    ///     bit_image_entwining [bit_image_entwining, bitwin]: number of passes for each bit image row.
    /// </summary>
    public int? BitImageEntwining => GetNum(TermInfoCaps.Num.BitImageEntwining);

    /// <summary>
    ///     bit_image_type [bit_image_type, bitype]: type of bit image device.
    /// </summary>
    public int? BitImageType => GetNum(TermInfoCaps.Num.BitImageType);

    /// <summary>
    ///     magic_cookie_glitch_ul [magic_cookie_glitch_ul, OTug]: number of blanks left by ul.
    /// </summary>
    public int? MagicCookieGlitchUl => GetNum(TermInfoCaps.Num.MagicCookieGlitchUl);

    /// <summary>
    ///     carriage_return_delay [carriage_return_delay, OTdC]: pad needed for cr.
    /// </summary>
    public int? CarriageReturnDelay => GetNum(TermInfoCaps.Num.CarriageReturnDelay);

    /// <summary>
    ///     new_line_delay [new_line_delay, OTdN]: pad needed for lf.
    /// </summary>
    public int? NewLineDelay => GetNum(TermInfoCaps.Num.NewLineDelay);

    /// <summary>
    ///     backspace_delay [backspace_delay, OTdB]: padding required for h.
    /// </summary>
    public int? BackspaceDelay => GetNum(TermInfoCaps.Num.BackspaceDelay);

    /// <summary>
    ///     horizontal_tab_delay [horizontal_tab_delay, OTdT]: padding required for i.
    /// </summary>
    public int? HorizontalTabDelay => GetNum(TermInfoCaps.Num.HorizontalTabDelay);

    /// <summary>
    ///     number_of_function_keys [number_of_function_keys, OTkn]: count of function keys.
    /// </summary>
    public int? NumberOfFunctionKeys => GetNum(TermInfoCaps.Num.NumberOfFunctionKeys);

    /// <summary>
    ///     back_tab [back_tab, cbt]: the back tab p.
    /// </summary>
    public string BackTab => GetString(TermInfoCaps.String.BackTab);

    /// <summary>
    ///     bell [bell, bel]: the audible signal bell p.
    /// </summary>
    public string Bell => GetString(TermInfoCaps.String.Bell);

    /// <summary>
    ///     carriage_return [carriage_return, cr]: the carriage return p p.
    /// </summary>
    public string CarriageReturn => GetString(TermInfoCaps.String.CarriageReturn);

    /// <summary>
    ///     change_scroll_region [change_scroll_region, csr]: the change region to line 1 to line 2 p.
    /// </summary>
    public string ChangeScrollRegion => GetString(TermInfoCaps.String.ChangeScrollRegion);

    /// <summary>
    ///     clear_all_tabs [clear_all_tabs, tbc]: the clear all tab stops p.
    /// </summary>
    public string ClearAllTabs => GetString(TermInfoCaps.String.ClearAllTabs);

    /// <summary>
    ///     clear_screen [clear_screen, clear]: the clear screen and home cursor p.
    /// </summary>
    public string ClearScreen => GetString(TermInfoCaps.String.ClearScreen);

    /// <summary>
    ///     clr_eol [clr_eol, el]: the clear to end of line p.
    /// </summary>
    public string ClrEol => GetString(TermInfoCaps.String.ClrEol);

    /// <summary>
    ///     clr_eos [clr_eos, ed]: the clear to end of screen p.
    /// </summary>
    public string ClrEos => GetString(TermInfoCaps.String.ClrEos);

    /// <summary>
    ///     column_address [column_address, hpa]: the horizontal position 1 absolute p.
    /// </summary>
    public string ColumnAddress => GetString(TermInfoCaps.String.ColumnAddress);

    /// <summary>
    ///     command_character [command_character, cmdch]: the terminal settable cmd character in prototype.
    /// </summary>
    public string CommandCharacter => GetString(TermInfoCaps.String.CommandCharacter);

    /// <summary>
    ///     cursor_address [cursor_address, cup]: the move to row 1 columns 2.
    /// </summary>
    public string CursorAddress => GetString(TermInfoCaps.String.CursorAddress);

    /// <summary>
    ///     cursor_down [cursor_down, cud1]: the down one line.
    /// </summary>
    public string CursorDown => GetString(TermInfoCaps.String.CursorDown);

    /// <summary>
    ///     cursor_home [cursor_home, home]: the home cursor if no cup.
    /// </summary>
    public string CursorHome => GetString(TermInfoCaps.String.CursorHome);

    /// <summary>
    ///     cursor_invisible [cursor_invisible, civis]: the make cursor invisible.
    /// </summary>
    public string CursorInvisible => GetString(TermInfoCaps.String.CursorInvisible);

    /// <summary>
    ///     cursor_left [cursor_left, cub1]: the move left one space.
    /// </summary>
    public string CursorLeft => GetString(TermInfoCaps.String.CursorLeft);

    /// <summary>
    ///     cursor_mem_address [cursor_mem_address, mrcup]: the memory relative cursor addressing move to row 1 columns 2.
    /// </summary>
    public string CursorMemAddress => GetString(TermInfoCaps.String.CursorMemAddress);

    /// <summary>
    ///     cursor_normal [cursor_normal, cnorm]: the make cursor appear normal undo civis cvvis.
    /// </summary>
    public string CursorNormal => GetString(TermInfoCaps.String.CursorNormal);

    /// <summary>
    ///     cursor_right [cursor_right, cuf1]: the non destructive space (move right one space).
    /// </summary>
    public string CursorRight => GetString(TermInfoCaps.String.CursorRight);

    /// <summary>
    ///     cursor_to_ll [cursor_to_ll, ll]: the last line first column if no cup.
    /// </summary>
    public string CursorToLl => GetString(TermInfoCaps.String.CursorToLl);

    /// <summary>
    ///     cursor_up [cursor_up, cuu1]: the up one line.
    /// </summary>
    public string CursorUp => GetString(TermInfoCaps.String.CursorUp);

    /// <summary>
    ///     cursor_visible [cursor_visible, cvvis]: the make cursor very visible.
    /// </summary>
    public string CursorVisible => GetString(TermInfoCaps.String.CursorVisible);

    /// <summary>
    ///     delete_character [delete_character, dch1]: the delete character p.
    /// </summary>
    public string DeleteCharacter => GetString(TermInfoCaps.String.DeleteCharacter);

    /// <summary>
    ///     delete_line [delete_line, dl1]: the delete line p.
    /// </summary>
    public string DeleteLine => GetString(TermInfoCaps.String.DeleteLine);

    /// <summary>
    ///     dis_status_line [dis_status_line, dsl]: the disable status line.
    /// </summary>
    public string DisStatusLine => GetString(TermInfoCaps.String.DisStatusLine);

    /// <summary>
    ///     down_half_line [down_half_line, hd]: the half a line down.
    /// </summary>
    public string DownHalfLine => GetString(TermInfoCaps.String.DownHalfLine);

    /// <summary>
    ///     enter_alt_charset_mode [enter_alt_charset_mode, smacs]: the start alternate character set p.
    /// </summary>
    public string EnterAltCharsetMode => GetString(TermInfoCaps.String.EnterAltCharsetMode);

    /// <summary>
    ///     enter_blink_mode [enter_blink_mode, blink]: the turn on blinking.
    /// </summary>
    public string EnterBlinkMode => GetString(TermInfoCaps.String.EnterBlinkMode);

    /// <summary>
    ///     enter_bold_mode [enter_bold_mode, bold]: the turn on bold extra bright mode.
    /// </summary>
    public string EnterBoldMode => GetString(TermInfoCaps.String.EnterBoldMode);

    /// <summary>
    ///     enter_ca_mode [enter_ca_mode, smcup]: the string to start programs using cup.
    /// </summary>
    public string EnterCaMode => GetString(TermInfoCaps.String.EnterCaMode);

    /// <summary>
    ///     enter_delete_mode [enter_delete_mode, smdc]: the enter delete mode.
    /// </summary>
    public string EnterDeleteMode => GetString(TermInfoCaps.String.EnterDeleteMode);

    /// <summary>
    ///     enter_dim_mode [enter_dim_mode, dim]: the turn on half bright mode.
    /// </summary>
    public string EnterDimMode => GetString(TermInfoCaps.String.EnterDimMode);

    /// <summary>
    ///     enter_insert_mode [enter_insert_mode, smir]: the enter insert mode.
    /// </summary>
    public string EnterInsertMode => GetString(TermInfoCaps.String.EnterInsertMode);

    /// <summary>
    ///     enter_secure_mode [enter_secure_mode, invis]: the turn on blank mode characters invisible.
    /// </summary>
    public string EnterSecureMode => GetString(TermInfoCaps.String.EnterSecureMode);

    /// <summary>
    ///     enter_protected_mode [enter_protected_mode, prot]: the turn on protected mode.
    /// </summary>
    public string EnterProtectedMode => GetString(TermInfoCaps.String.EnterProtectedMode);

    /// <summary>
    ///     enter_reverse_mode [enter_reverse_mode, rev]: the turn on reverse video mode.
    /// </summary>
    public string EnterReverseMode => GetString(TermInfoCaps.String.EnterReverseMode);

    /// <summary>
    ///     enter_standout_mode [enter_standout_mode, smso]: the begin standout mode.
    /// </summary>
    public string EnterStandoutMode => GetString(TermInfoCaps.String.EnterStandoutMode);

    /// <summary>
    ///     enter_underline_mode [enter_underline_mode, smul]: the begin underline mode.
    /// </summary>
    public string EnterUnderlineMode => GetString(TermInfoCaps.String.EnterUnderlineMode);

    /// <summary>
    ///     erase_chars [erase_chars, ech]: the erase 1 characters p.
    /// </summary>
    public string EraseChars => GetString(TermInfoCaps.String.EraseChars);

    /// <summary>
    ///     exit_alt_charset_mode [exit_alt_charset_mode, rmacs]: the end alternate character set p.
    /// </summary>
    public string ExitAltCharsetMode => GetString(TermInfoCaps.String.ExitAltCharsetMode);

    /// <summary>
    ///     exit_attribute_mode [exit_attribute_mode, sgr0]: the turn off all attributes.
    /// </summary>
    public string ExitAttributeMode => GetString(TermInfoCaps.String.ExitAttributeMode);

    /// <summary>
    ///     exit_ca_mode [exit_ca_mode, rmcup]: the strings to end programs using cup.
    /// </summary>
    public string ExitCaMode => GetString(TermInfoCaps.String.ExitCaMode);

    /// <summary>
    ///     exit_delete_mode [exit_delete_mode, rmdc]: the end delete mode.
    /// </summary>
    public string ExitDeleteMode => GetString(TermInfoCaps.String.ExitDeleteMode);

    /// <summary>
    ///     exit_insert_mode [exit_insert_mode, rmir]: the exit insert mode.
    /// </summary>
    public string ExitInsertMode => GetString(TermInfoCaps.String.ExitInsertMode);

    /// <summary>
    ///     exit_standout_mode [exit_standout_mode, rmso]: the exit standout mode.
    /// </summary>
    public string ExitStandoutMode => GetString(TermInfoCaps.String.ExitStandoutMode);

    /// <summary>
    ///     exit_underline_mode [exit_underline_mode, rmul]: the exit underline mode.
    /// </summary>
    public string ExitUnderlineMode => GetString(TermInfoCaps.String.ExitUnderlineMode);

    /// <summary>
    ///     flash_screen [flash_screen, flash]: the visible bell may not move cursor.
    /// </summary>
    public string FlashScreen => GetString(TermInfoCaps.String.FlashScreen);

    /// <summary>
    ///     form_feed [form_feed, ff]: the hardcopy terminal page eject p.
    /// </summary>
    public string FormFeed => GetString(TermInfoCaps.String.FormFeed);

    /// <summary>
    ///     from_status_line [from_status_line, fsl]: the return from status line.
    /// </summary>
    public string FromStatusLine => GetString(TermInfoCaps.String.FromStatusLine);

    /// <summary>
    ///     init_1string [init_1string, is1]: the initialization string.
    /// </summary>
    public string Init1string => GetString(TermInfoCaps.String.Init1string);

    /// <summary>
    ///     init_2string [init_2string, is2]: the initialization string.
    /// </summary>
    public string Init2string => GetString(TermInfoCaps.String.Init2string);

    /// <summary>
    ///     init_3string [init_3string, is3]: the initialization string.
    /// </summary>
    public string Init3string => GetString(TermInfoCaps.String.Init3string);

    /// <summary>
    ///     init_file [init_file, if]: the name of initialization file.
    /// </summary>
    public string InitFile => GetString(TermInfoCaps.String.InitFile);

    /// <summary>
    ///     insert_character [insert_character, ich1]: the insert character p.
    /// </summary>
    public string InsertCharacter => GetString(TermInfoCaps.String.InsertCharacter);

    /// <summary>
    ///     insert_line [insert_line, il1]: the insert line p.
    /// </summary>
    public string InsertLine => GetString(TermInfoCaps.String.InsertLine);

    /// <summary>
    ///     insert_padding [insert_padding, ip]: the insert padding after inserted character.
    /// </summary>
    public string InsertPadding => GetString(TermInfoCaps.String.InsertPadding);

    /// <summary>
    ///     key_backspace [key_backspace, kbs]: the backspace key.
    /// </summary>
    public string KeyBackspace => GetString(TermInfoCaps.String.KeyBackspace);

    /// <summary>
    ///     key_catab [key_catab, ktbc]: the clear all tabs key.
    /// </summary>
    public string KeyCatab => GetString(TermInfoCaps.String.KeyCatab);

    /// <summary>
    ///     key_clear [key_clear, kclr]: the clear screen or erase key.
    /// </summary>
    public string KeyClear => GetString(TermInfoCaps.String.KeyClear);

    /// <summary>
    ///     key_ctab [key_ctab, kctab]: the clear tab key.
    /// </summary>
    public string KeyCtab => GetString(TermInfoCaps.String.KeyCtab);

    /// <summary>
    ///     key_dc [key_dc, kdch1]: the delete character key.
    /// </summary>
    public string KeyDc => GetString(TermInfoCaps.String.KeyDc);

    /// <summary>
    ///     key_dl [key_dl, kdl1]: the delete line key.
    /// </summary>
    public string KeyDl => GetString(TermInfoCaps.String.KeyDl);

    /// <summary>
    ///     key_down [key_down, kcud1]: the down arrow key.
    /// </summary>
    public string KeyDown => GetString(TermInfoCaps.String.KeyDown);

    /// <summary>
    ///     key_eic [key_eic, krmir]: the sent by rmir or smir in insert mode.
    /// </summary>
    public string KeyEic => GetString(TermInfoCaps.String.KeyEic);

    /// <summary>
    ///     key_eol [key_eol, kel]: the clear to end of line key.
    /// </summary>
    public string KeyEol => GetString(TermInfoCaps.String.KeyEol);

    /// <summary>
    ///     key_eos [key_eos, ked]: the clear to end of screen key.
    /// </summary>
    public string KeyEos => GetString(TermInfoCaps.String.KeyEos);

    /// <summary>
    ///     key_f0 [key_f0, kf0]: the f 0 function key.
    /// </summary>
    public string KeyF0 => GetString(TermInfoCaps.String.KeyF0);

    /// <summary>
    ///     key_f1 [key_f1, kf1]: the f 1 function key.
    /// </summary>
    public string KeyF1 => GetString(TermInfoCaps.String.KeyF1);

    /// <summary>
    ///     key_f10 [key_f10, kf10]: the f 10 function key.
    /// </summary>
    public string KeyF10 => GetString(TermInfoCaps.String.KeyF10);

    /// <summary>
    ///     key_f2 [key_f2, kf2]: the f 2 function key.
    /// </summary>
    public string KeyF2 => GetString(TermInfoCaps.String.KeyF2);

    /// <summary>
    ///     key_f3 [key_f3, kf3]: the f 3 function key.
    /// </summary>
    public string KeyF3 => GetString(TermInfoCaps.String.KeyF3);

    /// <summary>
    ///     key_f4 [key_f4, kf4]: the f 4 function key.
    /// </summary>
    public string KeyF4 => GetString(TermInfoCaps.String.KeyF4);

    /// <summary>
    ///     key_f5 [key_f5, kf5]: the f 5 function key.
    /// </summary>
    public string KeyF5 => GetString(TermInfoCaps.String.KeyF5);

    /// <summary>
    ///     key_f6 [key_f6, kf6]: the f 6 function key.
    /// </summary>
    public string KeyF6 => GetString(TermInfoCaps.String.KeyF6);

    /// <summary>
    ///     key_f7 [key_f7, kf7]: the f 7 function key.
    /// </summary>
    public string KeyF7 => GetString(TermInfoCaps.String.KeyF7);

    /// <summary>
    ///     key_f8 [key_f8, kf8]: the f 8 function key.
    /// </summary>
    public string KeyF8 => GetString(TermInfoCaps.String.KeyF8);

    /// <summary>
    ///     key_f9 [key_f9, kf9]: the f 9 function key.
    /// </summary>
    public string KeyF9 => GetString(TermInfoCaps.String.KeyF9);

    /// <summary>
    ///     key_home [key_home, khome]: the home key.
    /// </summary>
    public string KeyHome => GetString(TermInfoCaps.String.KeyHome);

    /// <summary>
    ///     key_ic [key_ic, kich1]: the insert character key.
    /// </summary>
    public string KeyIc => GetString(TermInfoCaps.String.KeyIc);

    /// <summary>
    ///     key_il [key_il, kil1]: the insert line key.
    /// </summary>
    public string KeyIl => GetString(TermInfoCaps.String.KeyIl);

    /// <summary>
    ///     key_left [key_left, kcub1]: the left arrow key.
    /// </summary>
    public string KeyLeft => GetString(TermInfoCaps.String.KeyLeft);

    /// <summary>
    ///     key_ll [key_ll, kll]: the lower left key (home down).
    /// </summary>
    public string KeyLl => GetString(TermInfoCaps.String.KeyLl);

    /// <summary>
    ///     key_npage [key_npage, knp]: the next page key.
    /// </summary>
    public string KeyNpage => GetString(TermInfoCaps.String.KeyNpage);

    /// <summary>
    ///     key_ppage [key_ppage, kpp]: the previous page key.
    /// </summary>
    public string KeyPpage => GetString(TermInfoCaps.String.KeyPpage);

    /// <summary>
    ///     key_right [key_right, kcuf1]: the right arrow key.
    /// </summary>
    public string KeyRight => GetString(TermInfoCaps.String.KeyRight);

    /// <summary>
    ///     key_sf [key_sf, kind]: the scroll forward key.
    /// </summary>
    public string KeySf => GetString(TermInfoCaps.String.KeySf);

    /// <summary>
    ///     key_sr [key_sr, kri]: the scroll backward key.
    /// </summary>
    public string KeySr => GetString(TermInfoCaps.String.KeySr);

    /// <summary>
    ///     key_stab [key_stab, khts]: the set tab key.
    /// </summary>
    public string KeyStab => GetString(TermInfoCaps.String.KeyStab);

    /// <summary>
    ///     key_up [key_up, kcuu1]: the up arrow key.
    /// </summary>
    public string KeyUp => GetString(TermInfoCaps.String.KeyUp);

    /// <summary>
    ///     keypad_local [keypad_local, rmkx]: the leave 'keyboard transmit' mode.
    /// </summary>
    public string KeypadLocal => GetString(TermInfoCaps.String.KeypadLocal);

    /// <summary>
    ///     keypad_xmit [keypad_xmit, smkx]: the enter 'keyboard transmit' mode.
    /// </summary>
    public string KeypadXmit => GetString(TermInfoCaps.String.KeypadXmit);

    /// <summary>
    ///     lab_f0 [lab_f0, lf0]: the label on function key f 0 if not f 0.
    /// </summary>
    public string LabF0 => GetString(TermInfoCaps.String.LabF0);

    /// <summary>
    ///     lab_f1 [lab_f1, lf1]: the label on function key f 1 if not f 1.
    /// </summary>
    public string LabF1 => GetString(TermInfoCaps.String.LabF1);

    /// <summary>
    ///     lab_f10 [lab_f10, lf10]: the label on function key f 10 if not f 10.
    /// </summary>
    public string LabF10 => GetString(TermInfoCaps.String.LabF10);

    /// <summary>
    ///     lab_f2 [lab_f2, lf2]: the label on function key f 2 if not f 2.
    /// </summary>
    public string LabF2 => GetString(TermInfoCaps.String.LabF2);

    /// <summary>
    ///     lab_f3 [lab_f3, lf3]: the label on function key f 3 if not f 3.
    /// </summary>
    public string LabF3 => GetString(TermInfoCaps.String.LabF3);

    /// <summary>
    ///     lab_f4 [lab_f4, lf4]: the label on function key f 4 if not f 4.
    /// </summary>
    public string LabF4 => GetString(TermInfoCaps.String.LabF4);

    /// <summary>
    ///     lab_f5 [lab_f5, lf5]: the label on function key f 5 if not f 5.
    /// </summary>
    public string LabF5 => GetString(TermInfoCaps.String.LabF5);

    /// <summary>
    ///     lab_f6 [lab_f6, lf6]: the label on function key f 6 if not f 6.
    /// </summary>
    public string LabF6 => GetString(TermInfoCaps.String.LabF6);

    /// <summary>
    ///     lab_f7 [lab_f7, lf7]: the label on function key f 7 if not f 7.
    /// </summary>
    public string LabF7 => GetString(TermInfoCaps.String.LabF7);

    /// <summary>
    ///     lab_f8 [lab_f8, lf8]: the label on function key f 8 if not f 8.
    /// </summary>
    public string LabF8 => GetString(TermInfoCaps.String.LabF8);

    /// <summary>
    ///     lab_f9 [lab_f9, lf9]: the label on function key f 9 if not f 9.
    /// </summary>
    public string LabF9 => GetString(TermInfoCaps.String.LabF9);

    /// <summary>
    ///     meta_off [meta_off, rmm]: the turn off meta mode.
    /// </summary>
    public string MetaOff => GetString(TermInfoCaps.String.MetaOff);

    /// <summary>
    ///     meta_on [meta_on, smm]: the turn on meta mode (8th bit on).
    /// </summary>
    public string MetaOn => GetString(TermInfoCaps.String.MetaOn);

    /// <summary>
    ///     newline [newline, nel]: the newline behave like cr followed by lf.
    /// </summary>
    public string Newline => GetString(TermInfoCaps.String.Newline);

    /// <summary>
    ///     pad_char [pad_char, pad]: the padding char instead of null.
    /// </summary>
    public string PadChar => GetString(TermInfoCaps.String.PadChar);

    /// <summary>
    ///     parm_dch [parm_dch, dch]: the delete 1 characters p.
    /// </summary>
    public string ParmDch => GetString(TermInfoCaps.String.ParmDch);

    /// <summary>
    ///     parm_delete_line [parm_delete_line, dl]: the delete 1 lines p.
    /// </summary>
    public string ParmDeleteLine => GetString(TermInfoCaps.String.ParmDeleteLine);

    /// <summary>
    ///     parm_down_cursor [parm_down_cursor, cud]: the down 1 lines p.
    /// </summary>
    public string ParmDownCursor => GetString(TermInfoCaps.String.ParmDownCursor);

    /// <summary>
    ///     parm_ich [parm_ich, ich]: the insert 1 characters p.
    /// </summary>
    public string ParmIch => GetString(TermInfoCaps.String.ParmIch);

    /// <summary>
    ///     parm_index [parm_index, indn]: the scroll forward 1 lines p.
    /// </summary>
    public string ParmIndex => GetString(TermInfoCaps.String.ParmIndex);

    /// <summary>
    ///     parm_insert_line [parm_insert_line, il]: the insert 1 lines p.
    /// </summary>
    public string ParmInsertLine => GetString(TermInfoCaps.String.ParmInsertLine);

    /// <summary>
    ///     parm_left_cursor [parm_left_cursor, cub]: the move 1 characters to the left p.
    /// </summary>
    public string ParmLeftCursor => GetString(TermInfoCaps.String.ParmLeftCursor);

    /// <summary>
    ///     parm_right_cursor [parm_right_cursor, cuf]: the move 1 characters to the right p.
    /// </summary>
    public string ParmRightCursor => GetString(TermInfoCaps.String.ParmRightCursor);

    /// <summary>
    ///     parm_rindex [parm_rindex, rin]: the scroll back 1 lines p.
    /// </summary>
    public string ParmRindex => GetString(TermInfoCaps.String.ParmRindex);

    /// <summary>
    ///     parm_up_cursor [parm_up_cursor, cuu]: the up 1 lines p.
    /// </summary>
    public string ParmUpCursor => GetString(TermInfoCaps.String.ParmUpCursor);

    /// <summary>
    ///     pkey_key [pkey_key, pfkey]: the program function key 1 to type string 2.
    /// </summary>
    public string PkeyKey => GetString(TermInfoCaps.String.PkeyKey);

    /// <summary>
    ///     pkey_local [pkey_local, pfloc]: the program function key 1 to execute string 2.
    /// </summary>
    public string PkeyLocal => GetString(TermInfoCaps.String.PkeyLocal);

    /// <summary>
    ///     pkey_xmit [pkey_xmit, pfx]: the program function key 1 to transmit string 2.
    /// </summary>
    public string PkeyXmit => GetString(TermInfoCaps.String.PkeyXmit);

    /// <summary>
    ///     print_screen [print_screen, mc0]: the print contents of screen.
    /// </summary>
    public string PrintScreen => GetString(TermInfoCaps.String.PrintScreen);

    /// <summary>
    ///     prtr_off [prtr_off, mc4]: the turn off printer.
    /// </summary>
    public string PrtrOff => GetString(TermInfoCaps.String.PrtrOff);

    /// <summary>
    ///     prtr_on [prtr_on, mc5]: the turn on printer.
    /// </summary>
    public string PrtrOn => GetString(TermInfoCaps.String.PrtrOn);

    /// <summary>
    ///     repeat_char [repeat_char, rep]: the repeat char 1 2 times p.
    /// </summary>
    public string RepeatChar => GetString(TermInfoCaps.String.RepeatChar);

    /// <summary>
    ///     reset_1string [reset_1string, rs1]: the reset string.
    /// </summary>
    public string Reset1string => GetString(TermInfoCaps.String.Reset1string);

    /// <summary>
    ///     reset_2string [reset_2string, rs2]: the reset string.
    /// </summary>
    public string Reset2string => GetString(TermInfoCaps.String.Reset2string);

    /// <summary>
    ///     reset_3string [reset_3string, rs3]: the reset string.
    /// </summary>
    public string Reset3string => GetString(TermInfoCaps.String.Reset3string);

    /// <summary>
    ///     reset_file [reset_file, rf]: the name of reset file.
    /// </summary>
    public string ResetFile => GetString(TermInfoCaps.String.ResetFile);

    /// <summary>
    ///     restore_cursor [restore_cursor, rc]: the restore cursor to position of last save cursor.
    /// </summary>
    public string RestoreCursor => GetString(TermInfoCaps.String.RestoreCursor);

    /// <summary>
    ///     row_address [row_address, vpa]: the vertical position 1 absolute p.
    /// </summary>
    public string RowAddress => GetString(TermInfoCaps.String.RowAddress);

    /// <summary>
    ///     save_cursor [save_cursor, sc]: the save current cursor position p.
    /// </summary>
    public string SaveCursor => GetString(TermInfoCaps.String.SaveCursor);

    /// <summary>
    ///     scroll_forward [scroll_forward, ind]: the scroll text up p.
    /// </summary>
    public string ScrollForward => GetString(TermInfoCaps.String.ScrollForward);

    /// <summary>
    ///     scroll_reverse [scroll_reverse, ri]: the scroll text down p.
    /// </summary>
    public string ScrollReverse => GetString(TermInfoCaps.String.ScrollReverse);

    /// <summary>
    ///     set_attributes [set_attributes, sgr]: the define video attributes #1 #9 (pg9).
    /// </summary>
    public string SetAttributes => GetString(TermInfoCaps.String.SetAttributes);

    /// <summary>
    ///     set_tab [set_tab, hts]: the set a tab in every row current columns.
    /// </summary>
    public string SetTab => GetString(TermInfoCaps.String.SetTab);

    /// <summary>
    ///     set_window [set_window, wind]: the current window is lines #1 #2 cols #3 #4.
    /// </summary>
    public string SetWindow => GetString(TermInfoCaps.String.SetWindow);

    /// <summary>
    ///     tab [tab, ht]: the tab to next 8 space hardware tab stop.
    /// </summary>
    public string Tab => GetString(TermInfoCaps.String.Tab);

    /// <summary>
    ///     to_status_line [to_status_line, tsl]: the move to status line column 1.
    /// </summary>
    public string ToStatusLine => GetString(TermInfoCaps.String.ToStatusLine);

    /// <summary>
    ///     underline_char [underline_char, uc]: the underline char and move past it.
    /// </summary>
    public string UnderlineChar => GetString(TermInfoCaps.String.UnderlineChar);

    /// <summary>
    ///     up_half_line [up_half_line, hu]: the half a line up.
    /// </summary>
    public string UpHalfLine => GetString(TermInfoCaps.String.UpHalfLine);

    /// <summary>
    ///     init_prog [init_prog, iprog]: the path name of program for initialization.
    /// </summary>
    public string InitProg => GetString(TermInfoCaps.String.InitProg);

    /// <summary>
    ///     key_a1 [key_a1, ka1]: the upper left of keypad.
    /// </summary>
    public string KeyA1 => GetString(TermInfoCaps.String.KeyA1);

    /// <summary>
    ///     key_a3 [key_a3, ka3]: the upper right of keypad.
    /// </summary>
    public string KeyA3 => GetString(TermInfoCaps.String.KeyA3);

    /// <summary>
    ///     key_b2 [key_b2, kb2]: the center of keypad.
    /// </summary>
    public string KeyB2 => GetString(TermInfoCaps.String.KeyB2);

    /// <summary>
    ///     key_c1 [key_c1, kc1]: the lower left of keypad.
    /// </summary>
    public string KeyC1 => GetString(TermInfoCaps.String.KeyC1);

    /// <summary>
    ///     key_c3 [key_c3, kc3]: the lower right of keypad.
    /// </summary>
    public string KeyC3 => GetString(TermInfoCaps.String.KeyC3);

    /// <summary>
    ///     prtr_non [prtr_non, mc5p]: the turn on printer for 1 bytes.
    /// </summary>
    public string PrtrNon => GetString(TermInfoCaps.String.PrtrNon);

    /// <summary>
    ///     char_padding [char_padding, rmp]: the like ip but when in insert mode.
    /// </summary>
    public string CharPadding => GetString(TermInfoCaps.String.CharPadding);

    /// <summary>
    ///     acs_chars [acs_chars, acsc]: the graphics charset pairs based on vt 100.
    /// </summary>
    public string AcsChars => GetString(TermInfoCaps.String.AcsChars);

    /// <summary>
    ///     plab_norm [plab_norm, pln]: the program label 1 to show string 2.
    /// </summary>
    public string PlabNorm => GetString(TermInfoCaps.String.PlabNorm);

    /// <summary>
    ///     key_btab [key_btab, kcbt]: the back tab key.
    /// </summary>
    public string KeyBtab => GetString(TermInfoCaps.String.KeyBtab);

    /// <summary>
    ///     enter_xon_mode [enter_xon_mode, smxon]: the turn on xon xoff handshaking.
    /// </summary>
    public string EnterXonMode => GetString(TermInfoCaps.String.EnterXonMode);

    /// <summary>
    ///     exit_xon_mode [exit_xon_mode, rmxon]: the turn off xon xoff handshaking.
    /// </summary>
    public string ExitXonMode => GetString(TermInfoCaps.String.ExitXonMode);

    /// <summary>
    ///     enter_am_mode [enter_am_mode, smam]: the turn on automatic margins.
    /// </summary>
    public string EnterAmMode => GetString(TermInfoCaps.String.EnterAmMode);

    /// <summary>
    ///     exit_am_mode [exit_am_mode, rmam]: the turn off automatic margins.
    /// </summary>
    public string ExitAmMode => GetString(TermInfoCaps.String.ExitAmMode);

    /// <summary>
    ///     xon_character [xon_character, xonc]: the xon character.
    /// </summary>
    public string XonCharacter => GetString(TermInfoCaps.String.XonCharacter);

    /// <summary>
    ///     xoff_character [xoff_character, xoffc]: the xoff character.
    /// </summary>
    public string XoffCharacter => GetString(TermInfoCaps.String.XoffCharacter);

    /// <summary>
    ///     ena_acs [ena_acs, enacs]: the enable alternate char set.
    /// </summary>
    public string EnaAcs => GetString(TermInfoCaps.String.EnaAcs);

    /// <summary>
    ///     label_on [label_on, smln]: the turn on soft labels.
    /// </summary>
    public string LabelOn => GetString(TermInfoCaps.String.LabelOn);

    /// <summary>
    ///     label_off [label_off, rmln]: the turn off soft labels.
    /// </summary>
    public string LabelOff => GetString(TermInfoCaps.String.LabelOff);

    /// <summary>
    ///     key_beg [key_beg, kbeg]: the begin key.
    /// </summary>
    public string KeyBeg => GetString(TermInfoCaps.String.KeyBeg);

    /// <summary>
    ///     key_cancel [key_cancel, kcan]: the cancel key.
    /// </summary>
    public string KeyCancel => GetString(TermInfoCaps.String.KeyCancel);

    /// <summary>
    ///     key_close [key_close, kclo]: the close key.
    /// </summary>
    public string KeyClose => GetString(TermInfoCaps.String.KeyClose);

    /// <summary>
    ///     key_command [key_command, kcmd]: the command key.
    /// </summary>
    public string KeyCommand => GetString(TermInfoCaps.String.KeyCommand);

    /// <summary>
    ///     key_copy [key_copy, kcpy]: the copy key.
    /// </summary>
    public string KeyCopy => GetString(TermInfoCaps.String.KeyCopy);

    /// <summary>
    ///     key_create [key_create, kcrt]: the create key.
    /// </summary>
    public string KeyCreate => GetString(TermInfoCaps.String.KeyCreate);

    /// <summary>
    ///     key_end [key_end, kend]: the end key.
    /// </summary>
    public string KeyEnd => GetString(TermInfoCaps.String.KeyEnd);

    /// <summary>
    ///     key_enter [key_enter, kent]: the enter send key.
    /// </summary>
    public string KeyEnter => GetString(TermInfoCaps.String.KeyEnter);

    /// <summary>
    ///     key_exit [key_exit, kext]: the exit key.
    /// </summary>
    public string KeyExit => GetString(TermInfoCaps.String.KeyExit);

    /// <summary>
    ///     key_find [key_find, kfnd]: the find key.
    /// </summary>
    public string KeyFind => GetString(TermInfoCaps.String.KeyFind);

    /// <summary>
    ///     key_help [key_help, khlp]: the help key.
    /// </summary>
    public string KeyHelp => GetString(TermInfoCaps.String.KeyHelp);

    /// <summary>
    ///     key_mark [key_mark, kmrk]: the mark key.
    /// </summary>
    public string KeyMark => GetString(TermInfoCaps.String.KeyMark);

    /// <summary>
    ///     key_message [key_message, kmsg]: the message key.
    /// </summary>
    public string KeyMessage => GetString(TermInfoCaps.String.KeyMessage);

    /// <summary>
    ///     key_move [key_move, kmov]: the move key.
    /// </summary>
    public string KeyMove => GetString(TermInfoCaps.String.KeyMove);

    /// <summary>
    ///     key_next [key_next, knxt]: the next key.
    /// </summary>
    public string KeyNext => GetString(TermInfoCaps.String.KeyNext);

    /// <summary>
    ///     key_open [key_open, kopn]: the open key.
    /// </summary>
    public string KeyOpen => GetString(TermInfoCaps.String.KeyOpen);

    /// <summary>
    ///     key_options [key_options, kopt]: the options key.
    /// </summary>
    public string KeyOptions => GetString(TermInfoCaps.String.KeyOptions);

    /// <summary>
    ///     key_previous [key_previous, kprv]: the previous key.
    /// </summary>
    public string KeyPrevious => GetString(TermInfoCaps.String.KeyPrevious);

    /// <summary>
    ///     key_print [key_print, kprt]: the print key.
    /// </summary>
    public string KeyPrint => GetString(TermInfoCaps.String.KeyPrint);

    /// <summary>
    ///     key_redo [key_redo, krdo]: the redo key.
    /// </summary>
    public string KeyRedo => GetString(TermInfoCaps.String.KeyRedo);

    /// <summary>
    ///     key_reference [key_reference, kref]: the reference key.
    /// </summary>
    public string KeyReference => GetString(TermInfoCaps.String.KeyReference);

    /// <summary>
    ///     key_refresh [key_refresh, krfr]: the refresh key.
    /// </summary>
    public string KeyRefresh => GetString(TermInfoCaps.String.KeyRefresh);

    /// <summary>
    ///     key_replace [key_replace, krpl]: the replace key.
    /// </summary>
    public string KeyReplace => GetString(TermInfoCaps.String.KeyReplace);

    /// <summary>
    ///     key_restart [key_restart, krst]: the restart key.
    /// </summary>
    public string KeyRestart => GetString(TermInfoCaps.String.KeyRestart);

    /// <summary>
    ///     key_resume [key_resume, kres]: the resume key.
    /// </summary>
    public string KeyResume => GetString(TermInfoCaps.String.KeyResume);

    /// <summary>
    ///     key_save [key_save, ksav]: the save key.
    /// </summary>
    public string KeySave => GetString(TermInfoCaps.String.KeySave);

    /// <summary>
    ///     key_suspend [key_suspend, kspd]: the suspend key.
    /// </summary>
    public string KeySuspend => GetString(TermInfoCaps.String.KeySuspend);

    /// <summary>
    ///     key_undo [key_undo, kund]: the undo key.
    /// </summary>
    public string KeyUndo => GetString(TermInfoCaps.String.KeyUndo);

    /// <summary>
    ///     key_sbeg [key_sbeg, kBEG]: the shifted begin key.
    /// </summary>
    public string KeySbeg => GetString(TermInfoCaps.String.KeySbeg);

    /// <summary>
    ///     key_scancel [key_scancel, kCAN]: the shifted cancel key.
    /// </summary>
    public string KeyScancel => GetString(TermInfoCaps.String.KeyScancel);

    /// <summary>
    ///     key_scommand [key_scommand, kCMD]: the shifted command key.
    /// </summary>
    public string KeyScommand => GetString(TermInfoCaps.String.KeyScommand);

    /// <summary>
    ///     key_scopy [key_scopy, kCPY]: the shifted copy key.
    /// </summary>
    public string KeyScopy => GetString(TermInfoCaps.String.KeyScopy);

    /// <summary>
    ///     key_screate [key_screate, kCRT]: the shifted create key.
    /// </summary>
    public string KeyScreate => GetString(TermInfoCaps.String.KeyScreate);

    /// <summary>
    ///     key_sdc [key_sdc, kDC]: the shifted delete character key.
    /// </summary>
    public string KeySdc => GetString(TermInfoCaps.String.KeySdc);

    /// <summary>
    ///     key_sdl [key_sdl, kDL]: the shifted delete line key.
    /// </summary>
    public string KeySdl => GetString(TermInfoCaps.String.KeySdl);

    /// <summary>
    ///     key_select [key_select, kslt]: the select key.
    /// </summary>
    public string KeySelect => GetString(TermInfoCaps.String.KeySelect);

    /// <summary>
    ///     key_send [key_send, kEND]: the shifted end key.
    /// </summary>
    public string KeySend => GetString(TermInfoCaps.String.KeySend);

    /// <summary>
    ///     key_seol [key_seol, kEOL]: the shifted clear to end of line key.
    /// </summary>
    public string KeySeol => GetString(TermInfoCaps.String.KeySeol);

    /// <summary>
    ///     key_sexit [key_sexit, kEXT]: the shifted exit key.
    /// </summary>
    public string KeySexit => GetString(TermInfoCaps.String.KeySexit);

    /// <summary>
    ///     key_sfind [key_sfind, kFND]: the shifted find key.
    /// </summary>
    public string KeySfind => GetString(TermInfoCaps.String.KeySfind);

    /// <summary>
    ///     key_shelp [key_shelp, kHLP]: the shifted help key.
    /// </summary>
    public string KeyShelp => GetString(TermInfoCaps.String.KeyShelp);

    /// <summary>
    ///     key_shome [key_shome, kHOM]: the shifted home key.
    /// </summary>
    public string KeyShome => GetString(TermInfoCaps.String.KeyShome);

    /// <summary>
    ///     key_sic [key_sic, kIC]: the shifted insert character key.
    /// </summary>
    public string KeySic => GetString(TermInfoCaps.String.KeySic);

    /// <summary>
    ///     key_sleft [key_sleft, kLFT]: the shifted left arrow key.
    /// </summary>
    public string KeySleft => GetString(TermInfoCaps.String.KeySleft);

    /// <summary>
    ///     key_smessage [key_smessage, kMSG]: the shifted message key.
    /// </summary>
    public string KeySmessage => GetString(TermInfoCaps.String.KeySmessage);

    /// <summary>
    ///     key_smove [key_smove, kMOV]: the shifted move key.
    /// </summary>
    public string KeySmove => GetString(TermInfoCaps.String.KeySmove);

    /// <summary>
    ///     key_snext [key_snext, kNXT]: the shifted next key.
    /// </summary>
    public string KeySnext => GetString(TermInfoCaps.String.KeySnext);

    /// <summary>
    ///     key_soptions [key_soptions, kOPT]: the shifted options key.
    /// </summary>
    public string KeySoptions => GetString(TermInfoCaps.String.KeySoptions);

    /// <summary>
    ///     key_sprevious [key_sprevious, kPRV]: the shifted previous key.
    /// </summary>
    public string KeySprevious => GetString(TermInfoCaps.String.KeySprevious);

    /// <summary>
    ///     key_sprint [key_sprint, kPRT]: the shifted print key.
    /// </summary>
    public string KeySprint => GetString(TermInfoCaps.String.KeySprint);

    /// <summary>
    ///     key_sredo [key_sredo, kRDO]: the shifted redo key.
    /// </summary>
    public string KeySredo => GetString(TermInfoCaps.String.KeySredo);

    /// <summary>
    ///     key_sreplace [key_sreplace, kRPL]: the shifted replace key.
    /// </summary>
    public string KeySreplace => GetString(TermInfoCaps.String.KeySreplace);

    /// <summary>
    ///     key_sright [key_sright, kRIT]: the shifted right arrow key.
    /// </summary>
    public string KeySright => GetString(TermInfoCaps.String.KeySright);

    /// <summary>
    ///     key_srsume [key_srsume, kRES]: the shifted resume key.
    /// </summary>
    public string KeySrsume => GetString(TermInfoCaps.String.KeySrsume);

    /// <summary>
    ///     key_ssave [key_ssave, kSAV]: the shifted save key.
    /// </summary>
    public string KeySsave => GetString(TermInfoCaps.String.KeySsave);

    /// <summary>
    ///     key_ssuspend [key_ssuspend, kSPD]: the shifted suspend key.
    /// </summary>
    public string KeySsuspend => GetString(TermInfoCaps.String.KeySsuspend);

    /// <summary>
    ///     key_sundo [key_sundo, kUND]: the shifted undo key.
    /// </summary>
    public string KeySundo => GetString(TermInfoCaps.String.KeySundo);

    /// <summary>
    ///     req_for_input [req_for_input, rfi]: the send next input char for ptys.
    /// </summary>
    public string ReqForInput => GetString(TermInfoCaps.String.ReqForInput);

    /// <summary>
    ///     key_f11 [key_f11, kf11]: the f 11 function key.
    /// </summary>
    public string KeyF11 => GetString(TermInfoCaps.String.KeyF11);

    /// <summary>
    ///     key_f12 [key_f12, kf12]: the f 12 function key.
    /// </summary>
    public string KeyF12 => GetString(TermInfoCaps.String.KeyF12);

    /// <summary>
    ///     key_f13 [key_f13, kf13]: the f 13 function key.
    /// </summary>
    public string KeyF13 => GetString(TermInfoCaps.String.KeyF13);

    /// <summary>
    ///     key_f14 [key_f14, kf14]: the f 14 function key.
    /// </summary>
    public string KeyF14 => GetString(TermInfoCaps.String.KeyF14);

    /// <summary>
    ///     key_f15 [key_f15, kf15]: the f 15 function key.
    /// </summary>
    public string KeyF15 => GetString(TermInfoCaps.String.KeyF15);

    /// <summary>
    ///     key_f16 [key_f16, kf16]: the f 16 function key.
    /// </summary>
    public string KeyF16 => GetString(TermInfoCaps.String.KeyF16);

    /// <summary>
    ///     key_f17 [key_f17, kf17]: the f 17 function key.
    /// </summary>
    public string KeyF17 => GetString(TermInfoCaps.String.KeyF17);

    /// <summary>
    ///     key_f18 [key_f18, kf18]: the f 18 function key.
    /// </summary>
    public string KeyF18 => GetString(TermInfoCaps.String.KeyF18);

    /// <summary>
    ///     key_f19 [key_f19, kf19]: the f 19 function key.
    /// </summary>
    public string KeyF19 => GetString(TermInfoCaps.String.KeyF19);

    /// <summary>
    ///     key_f20 [key_f20, kf20]: the f 20 function key.
    /// </summary>
    public string KeyF20 => GetString(TermInfoCaps.String.KeyF20);

    /// <summary>
    ///     key_f21 [key_f21, kf21]: the f 21 function key.
    /// </summary>
    public string KeyF21 => GetString(TermInfoCaps.String.KeyF21);

    /// <summary>
    ///     key_f22 [key_f22, kf22]: the f 22 function key.
    /// </summary>
    public string KeyF22 => GetString(TermInfoCaps.String.KeyF22);

    /// <summary>
    ///     key_f23 [key_f23, kf23]: the f 23 function key.
    /// </summary>
    public string KeyF23 => GetString(TermInfoCaps.String.KeyF23);

    /// <summary>
    ///     key_f24 [key_f24, kf24]: the f 24 function key.
    /// </summary>
    public string KeyF24 => GetString(TermInfoCaps.String.KeyF24);

    /// <summary>
    ///     key_f25 [key_f25, kf25]: the f 25 function key.
    /// </summary>
    public string KeyF25 => GetString(TermInfoCaps.String.KeyF25);

    /// <summary>
    ///     key_f26 [key_f26, kf26]: the f 26 function key.
    /// </summary>
    public string KeyF26 => GetString(TermInfoCaps.String.KeyF26);

    /// <summary>
    ///     key_f27 [key_f27, kf27]: the f 27 function key.
    /// </summary>
    public string KeyF27 => GetString(TermInfoCaps.String.KeyF27);

    /// <summary>
    ///     key_f28 [key_f28, kf28]: the f 28 function key.
    /// </summary>
    public string KeyF28 => GetString(TermInfoCaps.String.KeyF28);

    /// <summary>
    ///     key_f29 [key_f29, kf29]: the f 29 function key.
    /// </summary>
    public string KeyF29 => GetString(TermInfoCaps.String.KeyF29);

    /// <summary>
    ///     key_f30 [key_f30, kf30]: the f 30 function key.
    /// </summary>
    public string KeyF30 => GetString(TermInfoCaps.String.KeyF30);

    /// <summary>
    ///     key_f31 [key_f31, kf31]: the f 31 function key.
    /// </summary>
    public string KeyF31 => GetString(TermInfoCaps.String.KeyF31);

    /// <summary>
    ///     key_f32 [key_f32, kf32]: the f 32 function key.
    /// </summary>
    public string KeyF32 => GetString(TermInfoCaps.String.KeyF32);

    /// <summary>
    ///     key_f33 [key_f33, kf33]: the f 33 function key.
    /// </summary>
    public string KeyF33 => GetString(TermInfoCaps.String.KeyF33);

    /// <summary>
    ///     key_f34 [key_f34, kf34]: the f 34 function key.
    /// </summary>
    public string KeyF34 => GetString(TermInfoCaps.String.KeyF34);

    /// <summary>
    ///     key_f35 [key_f35, kf35]: the f 35 function key.
    /// </summary>
    public string KeyF35 => GetString(TermInfoCaps.String.KeyF35);

    /// <summary>
    ///     key_f36 [key_f36, kf36]: the f 36 function key.
    /// </summary>
    public string KeyF36 => GetString(TermInfoCaps.String.KeyF36);

    /// <summary>
    ///     key_f37 [key_f37, kf37]: the f 37 function key.
    /// </summary>
    public string KeyF37 => GetString(TermInfoCaps.String.KeyF37);

    /// <summary>
    ///     key_f38 [key_f38, kf38]: the f 38 function key.
    /// </summary>
    public string KeyF38 => GetString(TermInfoCaps.String.KeyF38);

    /// <summary>
    ///     key_f39 [key_f39, kf39]: the f 39 function key.
    /// </summary>
    public string KeyF39 => GetString(TermInfoCaps.String.KeyF39);

    /// <summary>
    ///     key_f40 [key_f40, kf40]: the f 40 function key.
    /// </summary>
    public string KeyF40 => GetString(TermInfoCaps.String.KeyF40);

    /// <summary>
    ///     key_f41 [key_f41, kf41]: the f 41 function key.
    /// </summary>
    public string KeyF41 => GetString(TermInfoCaps.String.KeyF41);

    /// <summary>
    ///     key_f42 [key_f42, kf42]: the f 42 function key.
    /// </summary>
    public string KeyF42 => GetString(TermInfoCaps.String.KeyF42);

    /// <summary>
    ///     key_f43 [key_f43, kf43]: the f 43 function key.
    /// </summary>
    public string KeyF43 => GetString(TermInfoCaps.String.KeyF43);

    /// <summary>
    ///     key_f44 [key_f44, kf44]: the f 44 function key.
    /// </summary>
    public string KeyF44 => GetString(TermInfoCaps.String.KeyF44);

    /// <summary>
    ///     key_f45 [key_f45, kf45]: the f 45 function key.
    /// </summary>
    public string KeyF45 => GetString(TermInfoCaps.String.KeyF45);

    /// <summary>
    ///     key_f46 [key_f46, kf46]: the f 46 function key.
    /// </summary>
    public string KeyF46 => GetString(TermInfoCaps.String.KeyF46);

    /// <summary>
    ///     key_f47 [key_f47, kf47]: the f 47 function key.
    /// </summary>
    public string KeyF47 => GetString(TermInfoCaps.String.KeyF47);

    /// <summary>
    ///     key_f48 [key_f48, kf48]: the f 48 function key.
    /// </summary>
    public string KeyF48 => GetString(TermInfoCaps.String.KeyF48);

    /// <summary>
    ///     key_f49 [key_f49, kf49]: the f 49 function key.
    /// </summary>
    public string KeyF49 => GetString(TermInfoCaps.String.KeyF49);

    /// <summary>
    ///     key_f50 [key_f50, kf50]: the f 50 function key.
    /// </summary>
    public string KeyF50 => GetString(TermInfoCaps.String.KeyF50);

    /// <summary>
    ///     key_f51 [key_f51, kf51]: the f 51 function key.
    /// </summary>
    public string KeyF51 => GetString(TermInfoCaps.String.KeyF51);

    /// <summary>
    ///     key_f52 [key_f52, kf52]: the f 52 function key.
    /// </summary>
    public string KeyF52 => GetString(TermInfoCaps.String.KeyF52);

    /// <summary>
    ///     key_f53 [key_f53, kf53]: the f 53 function key.
    /// </summary>
    public string KeyF53 => GetString(TermInfoCaps.String.KeyF53);

    /// <summary>
    ///     key_f54 [key_f54, kf54]: the f 54 function key.
    /// </summary>
    public string KeyF54 => GetString(TermInfoCaps.String.KeyF54);

    /// <summary>
    ///     key_f55 [key_f55, kf55]: the f 55 function key.
    /// </summary>
    public string KeyF55 => GetString(TermInfoCaps.String.KeyF55);

    /// <summary>
    ///     key_f56 [key_f56, kf56]: the f 56 function key.
    /// </summary>
    public string KeyF56 => GetString(TermInfoCaps.String.KeyF56);

    /// <summary>
    ///     key_f57 [key_f57, kf57]: the f 57 function key.
    /// </summary>
    public string KeyF57 => GetString(TermInfoCaps.String.KeyF57);

    /// <summary>
    ///     key_f58 [key_f58, kf58]: the f 58 function key.
    /// </summary>
    public string KeyF58 => GetString(TermInfoCaps.String.KeyF58);

    /// <summary>
    ///     key_f59 [key_f59, kf59]: the f 59 function key.
    /// </summary>
    public string KeyF59 => GetString(TermInfoCaps.String.KeyF59);

    /// <summary>
    ///     key_f60 [key_f60, kf60]: the f 60 function key.
    /// </summary>
    public string KeyF60 => GetString(TermInfoCaps.String.KeyF60);

    /// <summary>
    ///     key_f61 [key_f61, kf61]: the f 61 function key.
    /// </summary>
    public string KeyF61 => GetString(TermInfoCaps.String.KeyF61);

    /// <summary>
    ///     key_f62 [key_f62, kf62]: the f 62 function key.
    /// </summary>
    public string KeyF62 => GetString(TermInfoCaps.String.KeyF62);

    /// <summary>
    ///     key_f63 [key_f63, kf63]: the f 63 function key.
    /// </summary>
    public string KeyF63 => GetString(TermInfoCaps.String.KeyF63);

    /// <summary>
    ///     clr_bol [clr_bol, el1]: the clear to beginning of line.
    /// </summary>
    public string ClrBol => GetString(TermInfoCaps.String.ClrBol);

    /// <summary>
    ///     clear_margins [clear_margins, mgc]: the clear right and left soft margins.
    /// </summary>
    public string ClearMargins => GetString(TermInfoCaps.String.ClearMargins);

    /// <summary>
    ///     set_left_margin [set_left_margin, smgl]: the set left soft margin at current column.
    /// </summary>
    public string SetLeftMargin => GetString(TermInfoCaps.String.SetLeftMargin);

    /// <summary>
    ///     set_right_margin [set_right_margin, smgr]: the set right soft margin at current column.
    /// </summary>
    public string SetRightMargin => GetString(TermInfoCaps.String.SetRightMargin);

    /// <summary>
    ///     label_format [label_format, fln]: the label format.
    /// </summary>
    public string LabelFormat => GetString(TermInfoCaps.String.LabelFormat);

    /// <summary>
    ///     set_clock [set_clock, sclk]: the set clock 1 hrs 2 mins 3 secs.
    /// </summary>
    public string SetClock => GetString(TermInfoCaps.String.SetClock);

    /// <summary>
    ///     display_clock [display_clock, dclk]: the display clock.
    /// </summary>
    public string DisplayClock => GetString(TermInfoCaps.String.DisplayClock);

    /// <summary>
    ///     remove_clock [remove_clock, rmclk]: the remove clock.
    /// </summary>
    public string RemoveClock => GetString(TermInfoCaps.String.RemoveClock);

    /// <summary>
    ///     create_window [create_window, cwin]: the define a window 1 from 2 3 to 4 5.
    /// </summary>
    public string CreateWindow => GetString(TermInfoCaps.String.CreateWindow);

    /// <summary>
    ///     goto_window [goto_window, wingo]: the go to window 1.
    /// </summary>
    public string GotoWindow => GetString(TermInfoCaps.String.GotoWindow);

    /// <summary>
    ///     hangup [hangup, hup]: the hang up phone.
    /// </summary>
    public string Hangup => GetString(TermInfoCaps.String.Hangup);

    /// <summary>
    ///     dial_phone [dial_phone, dial]: the dial number 1.
    /// </summary>
    public string DialPhone => GetString(TermInfoCaps.String.DialPhone);

    /// <summary>
    ///     quick_dial [quick_dial, qdial]: the dial number 1 without checking.
    /// </summary>
    public string QuickDial => GetString(TermInfoCaps.String.QuickDial);

    /// <summary>
    ///     tone [tone, tone]: the select touch tone dialing.
    /// </summary>
    public string Tone => GetString(TermInfoCaps.String.Tone);

    /// <summary>
    ///     pulse [pulse, pulse]: the select pulse dialing.
    /// </summary>
    public string Pulse => GetString(TermInfoCaps.String.Pulse);

    /// <summary>
    ///     flash_hook [flash_hook, hook]: the flash switch hook.
    /// </summary>
    public string FlashHook => GetString(TermInfoCaps.String.FlashHook);

    /// <summary>
    ///     fixed_pause [fixed_pause, pause]: the pause for 2 3 seconds.
    /// </summary>
    public string FixedPause => GetString(TermInfoCaps.String.FixedPause);

    /// <summary>
    ///     wait_tone [wait_tone, wait]: the wait for dial tone.
    /// </summary>
    public string WaitTone => GetString(TermInfoCaps.String.WaitTone);

    /// <summary>
    ///     user0 [user0, u0]: the user string 0.
    /// </summary>
    public string User0 => GetString(TermInfoCaps.String.User0);

    /// <summary>
    ///     user1 [user1, u1]: the user string 1.
    /// </summary>
    public string User1 => GetString(TermInfoCaps.String.User1);

    /// <summary>
    ///     user2 [user2, u2]: the user string 2.
    /// </summary>
    public string User2 => GetString(TermInfoCaps.String.User2);

    /// <summary>
    ///     user3 [user3, u3]: the user string 3.
    /// </summary>
    public string User3 => GetString(TermInfoCaps.String.User3);

    /// <summary>
    ///     user4 [user4, u4]: the user string 4.
    /// </summary>
    public string User4 => GetString(TermInfoCaps.String.User4);

    /// <summary>
    ///     user5 [user5, u5]: the user string 5.
    /// </summary>
    public string User5 => GetString(TermInfoCaps.String.User5);

    /// <summary>
    ///     user6 [user6, u6]: the user string 6.
    /// </summary>
    public string User6 => GetString(TermInfoCaps.String.User6);

    /// <summary>
    ///     user7 [user7, u7]: the user string 7.
    /// </summary>
    public string User7 => GetString(TermInfoCaps.String.User7);

    /// <summary>
    ///     user8 [user8, u8]: the user string 8.
    /// </summary>
    public string User8 => GetString(TermInfoCaps.String.User8);

    /// <summary>
    ///     user9 [user9, u9]: the user string 9.
    /// </summary>
    public string User9 => GetString(TermInfoCaps.String.User9);

    /// <summary>
    ///     orig_pair [orig_pair, op]: the set default pair to its original value.
    /// </summary>
    public string OrigPair => GetString(TermInfoCaps.String.OrigPair);

    /// <summary>
    ///     orig_colors [orig_colors, oc]: the set all color pairs to the original ones.
    /// </summary>
    public string OrigColors => GetString(TermInfoCaps.String.OrigColors);

    /// <summary>
    ///     initialize_color [initialize_color, initc]: the initialize color 1 to 2 3 4.
    /// </summary>
    public string InitializeColor => GetString(TermInfoCaps.String.InitializeColor);

    /// <summary>
    ///     initialize_pair [initialize_pair, initp]: the initialize color pair 1 to fg 2 3 4 bg 5 6 7.
    /// </summary>
    public string InitializePair => GetString(TermInfoCaps.String.InitializePair);

    /// <summary>
    ///     set_color_pair [set_color_pair, scp]: the set current color pair to 1.
    /// </summary>
    public string SetColorPair => GetString(TermInfoCaps.String.SetColorPair);

    /// <summary>
    ///     set_foreground [set_foreground, setf]: the set foreground color 1.
    /// </summary>
    public string SetForeground => GetString(TermInfoCaps.String.SetForeground);

    /// <summary>
    ///     set_background [set_background, setb]: the set background color 1.
    /// </summary>
    public string SetBackground => GetString(TermInfoCaps.String.SetBackground);

    /// <summary>
    ///     change_char_pitch [change_char_pitch, cpi]: the change number of characters per inch to 1.
    /// </summary>
    public string ChangeCharPitch => GetString(TermInfoCaps.String.ChangeCharPitch);

    /// <summary>
    ///     change_line_pitch [change_line_pitch, lpi]: the change number of lines per inch to 1.
    /// </summary>
    public string ChangeLinePitch => GetString(TermInfoCaps.String.ChangeLinePitch);

    /// <summary>
    ///     change_res_horz [change_res_horz, chr]: the change horizontal resolution to 1.
    /// </summary>
    public string ChangeResHorz => GetString(TermInfoCaps.String.ChangeResHorz);

    /// <summary>
    ///     change_res_vert [change_res_vert, cvr]: the change vertical resolution to 1.
    /// </summary>
    public string ChangeResVert => GetString(TermInfoCaps.String.ChangeResVert);

    /// <summary>
    ///     define_char [define_char, defc]: the define a character 1 2 dots wide descender 3.
    /// </summary>
    public string DefineChar => GetString(TermInfoCaps.String.DefineChar);

    /// <summary>
    ///     enter_doublewide_mode [enter_doublewide_mode, swidm]: the enter double wide mode.
    /// </summary>
    public string EnterDoublewideMode => GetString(TermInfoCaps.String.EnterDoublewideMode);

    /// <summary>
    ///     enter_draft_quality [enter_draft_quality, sdrfq]: the enter draft quality mode.
    /// </summary>
    public string EnterDraftQuality => GetString(TermInfoCaps.String.EnterDraftQuality);

    /// <summary>
    ///     enter_italics_mode [enter_italics_mode, sitm]: the enter italic mode.
    /// </summary>
    public string EnterItalicsMode => GetString(TermInfoCaps.String.EnterItalicsMode);

    /// <summary>
    ///     enter_leftward_mode [enter_leftward_mode, slm]: the start leftward carriage motion.
    /// </summary>
    public string EnterLeftwardMode => GetString(TermInfoCaps.String.EnterLeftwardMode);

    /// <summary>
    ///     enter_micro_mode [enter_micro_mode, smicm]: the start micro motion mode.
    /// </summary>
    public string EnterMicroMode => GetString(TermInfoCaps.String.EnterMicroMode);

    /// <summary>
    ///     enter_near_letter_quality [enter_near_letter_quality, snlq]: the enter nlq mode.
    /// </summary>
    public string EnterNearLetterQuality => GetString(TermInfoCaps.String.EnterNearLetterQuality);

    /// <summary>
    ///     enter_normal_quality [enter_normal_quality, snrmq]: the enter normal quality mode.
    /// </summary>
    public string EnterNormalQuality => GetString(TermInfoCaps.String.EnterNormalQuality);

    /// <summary>
    ///     enter_shadow_mode [enter_shadow_mode, sshm]: the enter shadow print mode.
    /// </summary>
    public string EnterShadowMode => GetString(TermInfoCaps.String.EnterShadowMode);

    /// <summary>
    ///     enter_subscript_mode [enter_subscript_mode, ssubm]: the enter subscript mode.
    /// </summary>
    public string EnterSubscriptMode => GetString(TermInfoCaps.String.EnterSubscriptMode);

    /// <summary>
    ///     enter_superscript_mode [enter_superscript_mode, ssupm]: the enter superscript mode.
    /// </summary>
    public string EnterSuperscriptMode => GetString(TermInfoCaps.String.EnterSuperscriptMode);

    /// <summary>
    ///     enter_upward_mode [enter_upward_mode, sum]: the start upward carriage motion.
    /// </summary>
    public string EnterUpwardMode => GetString(TermInfoCaps.String.EnterUpwardMode);

    /// <summary>
    ///     exit_doublewide_mode [exit_doublewide_mode, rwidm]: the end double wide mode.
    /// </summary>
    public string ExitDoublewideMode => GetString(TermInfoCaps.String.ExitDoublewideMode);

    /// <summary>
    ///     exit_italics_mode [exit_italics_mode, ritm]: the end italic mode.
    /// </summary>
    public string ExitItalicsMode => GetString(TermInfoCaps.String.ExitItalicsMode);

    /// <summary>
    ///     exit_leftward_mode [exit_leftward_mode, rlm]: the end left motion mode.
    /// </summary>
    public string ExitLeftwardMode => GetString(TermInfoCaps.String.ExitLeftwardMode);

    /// <summary>
    ///     exit_micro_mode [exit_micro_mode, rmicm]: the end micro motion mode.
    /// </summary>
    public string ExitMicroMode => GetString(TermInfoCaps.String.ExitMicroMode);

    /// <summary>
    ///     exit_shadow_mode [exit_shadow_mode, rshm]: the end shadow print mode.
    /// </summary>
    public string ExitShadowMode => GetString(TermInfoCaps.String.ExitShadowMode);

    /// <summary>
    ///     exit_subscript_mode [exit_subscript_mode, rsubm]: the end subscript mode.
    /// </summary>
    public string ExitSubscriptMode => GetString(TermInfoCaps.String.ExitSubscriptMode);

    /// <summary>
    ///     exit_superscript_mode [exit_superscript_mode, rsupm]: the end superscript mode.
    /// </summary>
    public string ExitSuperscriptMode => GetString(TermInfoCaps.String.ExitSuperscriptMode);

    /// <summary>
    ///     exit_upward_mode [exit_upward_mode, rum]: the end reverse character motion.
    /// </summary>
    public string ExitUpwardMode => GetString(TermInfoCaps.String.ExitUpwardMode);

    /// <summary>
    ///     micro_column_address [micro_column_address, mhpa]: the like column address in micro mode.
    /// </summary>
    public string MicroColumnAddress => GetString(TermInfoCaps.String.MicroColumnAddress);

    /// <summary>
    ///     micro_down [micro_down, mcud1]: the like cursor down in micro mode.
    /// </summary>
    public string MicroDown => GetString(TermInfoCaps.String.MicroDown);

    /// <summary>
    ///     micro_left [micro_left, mcub1]: the like cursor left in micro mode.
    /// </summary>
    public string MicroLeft => GetString(TermInfoCaps.String.MicroLeft);

    /// <summary>
    ///     micro_right [micro_right, mcuf1]: the like cursor right in micro mode.
    /// </summary>
    public string MicroRight => GetString(TermInfoCaps.String.MicroRight);

    /// <summary>
    ///     micro_row_address [micro_row_address, mvpa]: the like row address #1 in micro mode.
    /// </summary>
    public string MicroRowAddress => GetString(TermInfoCaps.String.MicroRowAddress);

    /// <summary>
    ///     micro_up [micro_up, mcuu1]: the like cursor up in micro mode.
    /// </summary>
    public string MicroUp => GetString(TermInfoCaps.String.MicroUp);

    /// <summary>
    ///     order_of_pins [order_of_pins, porder]: the match software bits to print head pins.
    /// </summary>
    public string OrderOfPins => GetString(TermInfoCaps.String.OrderOfPins);

    /// <summary>
    ///     parm_down_micro [parm_down_micro, mcud]: the like parm down cursor in micro mode.
    /// </summary>
    public string ParmDownMicro => GetString(TermInfoCaps.String.ParmDownMicro);

    /// <summary>
    ///     parm_left_micro [parm_left_micro, mcub]: the like parm left cursor in micro mode.
    /// </summary>
    public string ParmLeftMicro => GetString(TermInfoCaps.String.ParmLeftMicro);

    /// <summary>
    ///     parm_right_micro [parm_right_micro, mcuf]: the like parm right cursor in micro mode.
    /// </summary>
    public string ParmRightMicro => GetString(TermInfoCaps.String.ParmRightMicro);

    /// <summary>
    ///     parm_up_micro [parm_up_micro, mcuu]: the like parm up cursor in micro mode.
    /// </summary>
    public string ParmUpMicro => GetString(TermInfoCaps.String.ParmUpMicro);

    /// <summary>
    ///     select_char_set [select_char_set, scs]: the select character set 1.
    /// </summary>
    public string SelectCharSet => GetString(TermInfoCaps.String.SelectCharSet);

    /// <summary>
    ///     set_bottom_margin [set_bottom_margin, smgb]: the set bottom margin at current line.
    /// </summary>
    public string SetBottomMargin => GetString(TermInfoCaps.String.SetBottomMargin);

    /// <summary>
    ///     set_bottom_margin_parm [set_bottom_margin_parm, smgbp]: the set bottom margin at line 1 or if smgtp is not given 2 lines from bottom.
    /// </summary>
    public string SetBottomMarginParm => GetString(TermInfoCaps.String.SetBottomMarginParm);

    /// <summary>
    ///     set_left_margin_parm [set_left_margin_parm, smglp]: the set left right margin at column 1.
    /// </summary>
    public string SetLeftMarginParm => GetString(TermInfoCaps.String.SetLeftMarginParm);

    /// <summary>
    ///     set_right_margin_parm [set_right_margin_parm, smgrp]: the set right margin at column 1.
    /// </summary>
    public string SetRightMarginParm => GetString(TermInfoCaps.String.SetRightMarginParm);

    /// <summary>
    ///     set_top_margin [set_top_margin, smgt]: the set top margin at current line.
    /// </summary>
    public string SetTopMargin => GetString(TermInfoCaps.String.SetTopMargin);

    /// <summary>
    ///     set_top_margin_parm [set_top_margin_parm, smgtp]: the set top bottom margin at row 1.
    /// </summary>
    public string SetTopMarginParm => GetString(TermInfoCaps.String.SetTopMarginParm);

    /// <summary>
    ///     start_bit_image [start_bit_image, sbim]: the start printing bit image graphics.
    /// </summary>
    public string StartBitImage => GetString(TermInfoCaps.String.StartBitImage);

    /// <summary>
    ///     start_char_set_def [start_char_set_def, scsd]: the start character set definition 1 with 2 characters in the set.
    /// </summary>
    public string StartCharSetDef => GetString(TermInfoCaps.String.StartCharSetDef);

    /// <summary>
    ///     stop_bit_image [stop_bit_image, rbim]: the stop printing bit image graphics.
    /// </summary>
    public string StopBitImage => GetString(TermInfoCaps.String.StopBitImage);

    /// <summary>
    ///     stop_char_set_def [stop_char_set_def, rcsd]: the end definition of character set 1.
    /// </summary>
    public string StopCharSetDef => GetString(TermInfoCaps.String.StopCharSetDef);

    /// <summary>
    ///     subscript_characters [subscript_characters, subcs]: the list of subscriptable characters.
    /// </summary>
    public string SubscriptCharacters => GetString(TermInfoCaps.String.SubscriptCharacters);

    /// <summary>
    ///     superscript_characters [superscript_characters, supcs]: the list of superscriptable characters.
    /// </summary>
    public string SuperscriptCharacters => GetString(TermInfoCaps.String.SuperscriptCharacters);

    /// <summary>
    ///     these_cause_cr [these_cause_cr, docr]: the printing any of these characters causes cr.
    /// </summary>
    public string TheseCauseCr => GetString(TermInfoCaps.String.TheseCauseCr);

    /// <summary>
    ///     zero_motion [zero_motion, zerom]: the no motion for subsequent character.
    /// </summary>
    public string ZeroMotion => GetString(TermInfoCaps.String.ZeroMotion);

    /// <summary>
    ///     char_set_names [char_set_names, csnm]: the produce 1 th item from list of character set names.
    /// </summary>
    public string CharSetNames => GetString(TermInfoCaps.String.CharSetNames);

    /// <summary>
    ///     key_mouse [key_mouse, kmous]: the mouse event has occurred.
    /// </summary>
    public string KeyMouse => GetString(TermInfoCaps.String.KeyMouse);

    /// <summary>
    ///     mouse_info [mouse_info, minfo]: the mouse status information.
    /// </summary>
    public string MouseInfo => GetString(TermInfoCaps.String.MouseInfo);

    /// <summary>
    ///     req_mouse_pos [req_mouse_pos, reqmp]: the request mouse position.
    /// </summary>
    public string ReqMousePos => GetString(TermInfoCaps.String.ReqMousePos);

    /// <summary>
    ///     get_mouse [get_mouse, getm]: the curses should get button events parameter 1 not documented.
    /// </summary>
    public string GetMouse => GetString(TermInfoCaps.String.GetMouse);

    /// <summary>
    ///     set_a_foreground [set_a_foreground, setaf]: the set foreground color to 1 using ansi escape.
    /// </summary>
    public string SetAForeground => GetString(TermInfoCaps.String.SetAForeground);

    /// <summary>
    ///     set_a_background [set_a_background, setab]: the set background color to 1 using ansi escape.
    /// </summary>
    public string SetABackground => GetString(TermInfoCaps.String.SetABackground);

    /// <summary>
    ///     pkey_plab [pkey_plab, pfxl]: the program function key 1 to type string 2 and show string 3.
    /// </summary>
    public string PkeyPlab => GetString(TermInfoCaps.String.PkeyPlab);

    /// <summary>
    ///     device_type [device_type, devt]: the indicate language codeset support.
    /// </summary>
    public string DeviceType => GetString(TermInfoCaps.String.DeviceType);

    /// <summary>
    ///     code_set_init [code_set_init, csin]: the init sequence for multiple codesets.
    /// </summary>
    public string CodeSetInit => GetString(TermInfoCaps.String.CodeSetInit);

    /// <summary>
    ///     set0_des_seq [set0_des_seq, s0ds]: the shift to codeset 0 euc set 0 ascii.
    /// </summary>
    public string Set0DesSeq => GetString(TermInfoCaps.String.Set0DesSeq);

    /// <summary>
    ///     set1_des_seq [set1_des_seq, s1ds]: the shift to codeset 1.
    /// </summary>
    public string Set1DesSeq => GetString(TermInfoCaps.String.Set1DesSeq);

    /// <summary>
    ///     set2_des_seq [set2_des_seq, s2ds]: the shift to codeset 2.
    /// </summary>
    public string Set2DesSeq => GetString(TermInfoCaps.String.Set2DesSeq);

    /// <summary>
    ///     set3_des_seq [set3_des_seq, s3ds]: the shift to codeset 3.
    /// </summary>
    public string Set3DesSeq => GetString(TermInfoCaps.String.Set3DesSeq);

    /// <summary>
    ///     set_lr_margin [set_lr_margin, smglr]: the set both left and right margins to 1 2 ml is not in bsd termcap.
    /// </summary>
    public string SetLrMargin => GetString(TermInfoCaps.String.SetLrMargin);

    /// <summary>
    ///     set_tb_margin [set_tb_margin, smgtb]: the sets both top and bottom margins to 1 2.
    /// </summary>
    public string SetTbMargin => GetString(TermInfoCaps.String.SetTbMargin);

    /// <summary>
    ///     bit_image_repeat [bit_image_repeat, birep]: the repeat bit image cell 1 2 times.
    /// </summary>
    public string BitImageRepeat => GetString(TermInfoCaps.String.BitImageRepeat);

    /// <summary>
    ///     bit_image_newline [bit_image_newline, binel]: the move to next row of the bit image.
    /// </summary>
    public string BitImageNewline => GetString(TermInfoCaps.String.BitImageNewline);

    /// <summary>
    ///     bit_image_carriage_return [bit_image_carriage_return, bicr]: the move to beginning of same row.
    /// </summary>
    public string BitImageCarriageReturn => GetString(TermInfoCaps.String.BitImageCarriageReturn);

    /// <summary>
    ///     color_names [color_names, colornm]: the give name for color 1.
    /// </summary>
    public string ColorNames => GetString(TermInfoCaps.String.ColorNames);

    /// <summary>
    ///     define_bit_image_region [define_bit_image_region, defbi]: the define rectangular bit image region.
    /// </summary>
    public string DefineBitImageRegion => GetString(TermInfoCaps.String.DefineBitImageRegion);

    /// <summary>
    ///     end_bit_image_region [end_bit_image_region, endbi]: the end a bit image region.
    /// </summary>
    public string EndBitImageRegion => GetString(TermInfoCaps.String.EndBitImageRegion);

    /// <summary>
    ///     set_color_band [set_color_band, setcolor]: the change to ribbon color 1.
    /// </summary>
    public string SetColorBand => GetString(TermInfoCaps.String.SetColorBand);

    /// <summary>
    ///     set_page_length [set_page_length, slines]: the set page length to 1 lines.
    /// </summary>
    public string SetPageLength => GetString(TermInfoCaps.String.SetPageLength);

    /// <summary>
    ///     display_pc_char [display_pc_char, dispc]: the display pc character 1.
    /// </summary>
    public string DisplayPcChar => GetString(TermInfoCaps.String.DisplayPcChar);

    /// <summary>
    ///     enter_pc_charset_mode [enter_pc_charset_mode, smpch]: the enter pc character display mode.
    /// </summary>
    public string EnterPcCharsetMode => GetString(TermInfoCaps.String.EnterPcCharsetMode);

    /// <summary>
    ///     exit_pc_charset_mode [exit_pc_charset_mode, rmpch]: the exit pc character display mode.
    /// </summary>
    public string ExitPcCharsetMode => GetString(TermInfoCaps.String.ExitPcCharsetMode);

    /// <summary>
    ///     enter_scancode_mode [enter_scancode_mode, smsc]: the enter pc scancode mode.
    /// </summary>
    public string EnterScancodeMode => GetString(TermInfoCaps.String.EnterScancodeMode);

    /// <summary>
    ///     exit_scancode_mode [exit_scancode_mode, rmsc]: the exit pc scancode mode.
    /// </summary>
    public string ExitScancodeMode => GetString(TermInfoCaps.String.ExitScancodeMode);

    /// <summary>
    ///     pc_term_options [pc_term_options, pctrm]: the pc terminal options.
    /// </summary>
    public string PcTermOptions => GetString(TermInfoCaps.String.PcTermOptions);

    /// <summary>
    ///     scancode_escape [scancode_escape, scesc]: the escape for scancode emulation.
    /// </summary>
    public string ScancodeEscape => GetString(TermInfoCaps.String.ScancodeEscape);

    /// <summary>
    ///     alt_scancode_esc [alt_scancode_esc, scesa]: the alternate escape for scancode emulation.
    /// </summary>
    public string AltScancodeEsc => GetString(TermInfoCaps.String.AltScancodeEsc);

    /// <summary>
    ///     enter_horizontal_hl_mode [enter_horizontal_hl_mode, ehhlm]: the enter horizontal highlight mode.
    /// </summary>
    public string EnterHorizontalHlMode => GetString(TermInfoCaps.String.EnterHorizontalHlMode);

    /// <summary>
    ///     enter_left_hl_mode [enter_left_hl_mode, elhlm]: the enter left highlight mode.
    /// </summary>
    public string EnterLeftHlMode => GetString(TermInfoCaps.String.EnterLeftHlMode);

    /// <summary>
    ///     enter_low_hl_mode [enter_low_hl_mode, elohlm]: the enter low highlight mode.
    /// </summary>
    public string EnterLowHlMode => GetString(TermInfoCaps.String.EnterLowHlMode);

    /// <summary>
    ///     enter_right_hl_mode [enter_right_hl_mode, erhlm]: the enter right highlight mode.
    /// </summary>
    public string EnterRightHlMode => GetString(TermInfoCaps.String.EnterRightHlMode);

    /// <summary>
    ///     enter_top_hl_mode [enter_top_hl_mode, ethlm]: the enter top highlight mode.
    /// </summary>
    public string EnterTopHlMode => GetString(TermInfoCaps.String.EnterTopHlMode);

    /// <summary>
    ///     enter_vertical_hl_mode [enter_vertical_hl_mode, evhlm]: the enter vertical highlight mode.
    /// </summary>
    public string EnterVerticalHlMode => GetString(TermInfoCaps.String.EnterVerticalHlMode);

    /// <summary>
    ///     set_a_attributes [set_a_attributes, sgr1]: the define second set of video attributes #1 #6.
    /// </summary>
    public string SetAAttributes => GetString(TermInfoCaps.String.SetAAttributes);

    /// <summary>
    ///     set_pglen_inch [set_pglen_inch, slength]: the set page length to 1 hundredth of an inch some implementations use s l for termcap.
    /// </summary>
    public string SetPglenInch => GetString(TermInfoCaps.String.SetPglenInch);

    /// <summary>
    ///     termcap_init2 [termcap_init2, OTi2]: the secondary initialization string.
    /// </summary>
    public string TermcapInit2 => GetString(TermInfoCaps.String.TermcapInit2);

    /// <summary>
    ///     termcap_reset [termcap_reset, OTrs]: the terminal reset string.
    /// </summary>
    public string TermcapReset => GetString(TermInfoCaps.String.TermcapReset);

    /// <summary>
    ///     linefeed_if_not_lf [linefeed_if_not_lf, OTnl]: the use to move down.
    /// </summary>
    public string LinefeedIfNotLf => GetString(TermInfoCaps.String.LinefeedIfNotLf);

    /// <summary>
    ///     backspace_if_not_bs [backspace_if_not_bs, OTbc]: the move left if not h.
    /// </summary>
    public string BackspaceIfNotBs => GetString(TermInfoCaps.String.BackspaceIfNotBs);

    /// <summary>
    ///     other_non_function_keys [other_non_function_keys, OTko]: the list of self mapped keycaps.
    /// </summary>
    public string OtherNonFunctionKeys => GetString(TermInfoCaps.String.OtherNonFunctionKeys);

    /// <summary>
    ///     arrow_key_map [arrow_key_map, OTma]: the map motion keys for vi version 2.
    /// </summary>
    public string ArrowKeyMap => GetString(TermInfoCaps.String.ArrowKeyMap);

    /// <summary>
    ///     acs_ulcorner [acs_ulcorner, OTG2]: the single upper left.
    /// </summary>
    public string AcsUlcorner => GetString(TermInfoCaps.String.AcsUlcorner);

    /// <summary>
    ///     acs_llcorner [acs_llcorner, OTG3]: the single lower left.
    /// </summary>
    public string AcsLlcorner => GetString(TermInfoCaps.String.AcsLlcorner);

    /// <summary>
    ///     acs_urcorner [acs_urcorner, OTG1]: the single upper right.
    /// </summary>
    public string AcsUrcorner => GetString(TermInfoCaps.String.AcsUrcorner);

    /// <summary>
    ///     acs_lrcorner [acs_lrcorner, OTG4]: the single lower right.
    /// </summary>
    public string AcsLrcorner => GetString(TermInfoCaps.String.AcsLrcorner);

    /// <summary>
    ///     acs_ltee [acs_ltee, OTGR]: the tee pointing right.
    /// </summary>
    public string AcsLtee => GetString(TermInfoCaps.String.AcsLtee);

    /// <summary>
    ///     acs_rtee [acs_rtee, OTGL]: the tee pointing left.
    /// </summary>
    public string AcsRtee => GetString(TermInfoCaps.String.AcsRtee);

    /// <summary>
    ///     acs_btee [acs_btee, OTGU]: the tee pointing up.
    /// </summary>
    public string AcsBtee => GetString(TermInfoCaps.String.AcsBtee);

    /// <summary>
    ///     acs_ttee [acs_ttee, OTGD]: the tee pointing down.
    /// </summary>
    public string AcsTtee => GetString(TermInfoCaps.String.AcsTtee);

    /// <summary>
    ///     acs_hline [acs_hline, OTGH]: the single horizontal line.
    /// </summary>
    public string AcsHline => GetString(TermInfoCaps.String.AcsHline);

    /// <summary>
    ///     acs_vline [acs_vline, OTGV]: the single vertical line.
    /// </summary>
    public string AcsVline => GetString(TermInfoCaps.String.AcsVline);

    /// <summary>
    ///     acs_plus [acs_plus, OTGC]: the single intersection.
    /// </summary>
    public string AcsPlus => GetString(TermInfoCaps.String.AcsPlus);

    /// <summary>
    ///     memory_lock [memory_lock, meml]: the lock memory above cursor.
    /// </summary>
    public string MemoryLock => GetString(TermInfoCaps.String.MemoryLock);

    /// <summary>
    ///     memory_unlock [memory_unlock, memu]: the unlock memory.
    /// </summary>
    public string MemoryUnlock => GetString(TermInfoCaps.String.MemoryUnlock);

    /// <summary>
    ///     box_chars_1 [box_chars_1, box1]: the box characters primary set.
    /// </summary>
    public string BoxChars1 => GetString(TermInfoCaps.String.BoxChars1);
}