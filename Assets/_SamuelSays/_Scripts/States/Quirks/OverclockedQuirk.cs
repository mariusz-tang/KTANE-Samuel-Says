using System;
using System.Collections;
using UnityEngine;

using Rnd = UnityEngine.Random;

public class OverclockedQuirk : State {

    private const float DECREASE_RATE = 1f;
    private const float INCREMENT_SIZE = 0.25f;

    private float _spamTotal;
    private bool _hitLimit;

    private Coroutine _trackSpam;

    public OverclockedQuirk(SamuelSaysModule module) : base(module) { }

    public override IEnumerator OnStateEnter() {
        _module.Screen.PlaySequence(_module.DisplayedSequence, true);
        _spamTotal = 0;
        _hitLimit = false;

        _module.LogQuirk("Overclocked");
        _module.Log("Press buttons rapidly until Samuel calms down.");
        _trackSpam = _module.StartCoroutine(TrackSpam());
        yield return null;
    }

    public override IEnumerator HandlePress(ColouredButton button) {
        if (_hitLimit) {
            button.AddInteractionPunch();
            yield break;
        }

        button.PlayPressAnimation();
        _spamTotal = Math.Min(1, _spamTotal + INCREMENT_SIZE);
        if (_spamTotal == 1 && !_hitLimit) {
            _module.Log("Samuel is calm now.");
            _hitLimit = true;
            button.AddInteractionPunch();
            LightAllButtons();
        }
        yield return null;
    }

    public override IEnumerator HandleRelease(ColouredButton button) {
        if (!_hitLimit) {
            button.PlayReleaseAnimation();
        }
        yield return null;
    }

    private IEnumerator TrackSpam() {
        while (true) {
            _spamTotal = Math.Max(0, _spamTotal - Time.deltaTime * DECREASE_RATE);
            _module.SymbolDisplay.DisplayColour(Color.white * _spamTotal, string.Empty);

            if (_hitLimit && _spamTotal == 0) {
                yield return new WaitForSeconds(0.2f);
                TransitionToNextState();
                _module.StopCoroutine(_trackSpam);
            }
            yield return null;
        }
    }

    private void LightAllButtons() {
        foreach (ColouredButton button in _module.Buttons) {
            button.PlayPressAnimation();
        }
    }

    private void TransitionToNextState() {
        foreach (ColouredButton button in _module.Buttons) {
            button.PlayReleaseAnimation();
        }
        _module.ChangeState(new RegularStage(_module));
    }

    public override TpAction NextTpAction() {
        return new TpAction(TpActionType.PressShort, Rnd.Range(0, 4));
    }
}
