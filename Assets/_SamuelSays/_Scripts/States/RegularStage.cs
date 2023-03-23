using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegularStage : State {

    private ButtonColour _heldButtonColour;
    private float _timeHeld;
    private bool _isHolding;

    private ColouredSymbol[] _currentDisplaySequence;

    public RegularStage(SamuelSaysModule module) : base(module) { }

    public override IEnumerator OnStateEnter() {
        _currentDisplaySequence = _module.DisplayedSequences[0];
        _module.Screen.PlaySequence(_currentDisplaySequence);
        yield return null;
    }

    public override IEnumerator HandlePress(ColouredButton button) {
        button.PlayPressAnimation();
        _heldButtonColour = button.Colour;
        _timeHeld = 0;
        _module.StartCoroutine(TrackHoldTime());
        _module.SymbolDisplay.DisplayLetter('•');
        yield return null;
    }

    private IEnumerator TrackHoldTime() {
        _isHolding = true;

        while (_isHolding) {
            _timeHeld += Time.deltaTime;

            if (_timeHeld >= 0.5f) {
                _module.SymbolDisplay.DisplayLetter('ー');
            }
            yield return null;
        }
    }

    public override IEnumerator HandleRelease(ColouredButton button) {
        _isHolding = false;
        button.PlayReleaseAnimation();
        _module.SymbolDisplay.ClearScreen();

        char inputtedSymbol = _timeHeld >= 0.5f ? '-' : '.';
        var submittedSymbol = new ColouredSymbol(_heldButtonColour, inputtedSymbol);

        CheckSubmission(submittedSymbol);

        yield return null;
    }

    private void CheckSubmission(ColouredSymbol submittedSymbol) {
        if (submittedSymbol.Equals(_module.ExpectedSubmission)) {
            _module.Log("Correct submission!");
            _module.AdvanceStage();
            _module.ChangeState(new RegularStage(_module));
        }
        else {
            _module.Strike("Incorrected submitted a " + submittedSymbol.ToString() + "! Strike!");
        }
    }

    public override IEnumerator HandleSubmitPress() {
        _module.Log("Submitted a ");
        yield return null;
    }
}