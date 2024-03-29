﻿using System;

public class ColouredSymbol : IEquatable<ColouredSymbol> {

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
            return Colour.ToString().ToLower() + " dot";
        }
        return Colour.ToString().ToLower() + " dash";
    }

    public bool Equals(ColouredSymbol other) {
        return (Symbol == other.Symbol) && (Colour == other.Colour);
    }
}
