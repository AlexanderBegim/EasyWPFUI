// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using EasyWPFUI.Media.ColorPalette.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace EasyWPFUI.Media.ColorPalette
{
    public interface IColorPaletteEntry
    {
        string Title { get; }
        string Description { get; }

        Color ActiveColor { get; set; }
        string ActiveColorString { get; }

        ColorStringFormat ActiveColorStringFormat { get; }

        event Action<IColorPaletteEntry> ActiveColorChanged;

        IReadOnlyList<ContrastColorWrapper> ContrastColors { get; set; }
        ContrastColorWrapper BestContrastColor { get; }

        event Action<IColorPaletteEntry> ContrastColorChanged;
    }
}
