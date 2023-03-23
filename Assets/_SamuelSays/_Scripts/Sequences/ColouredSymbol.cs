﻿using System;

public class ColouredSymbol {

    public ButtonColour Colour { get; set; }
    public char Symbol { get; set; }

    public ColouredSymbol(ButtonColour colour, char symbol) {
        if ((symbol != '.') && (symbol != '-')) {
            throw new ArgumentException("Symbol must be either a dit '.' or a dah '-'.");
        }

        Colour = colour;
        Symbol = symbol;
    }

    public override string ToString() {
        if (Symbol == '.') {
            return Colour.ToString() + " dit";
        }
        return Colour.ToString() + " dah";
    }
}
