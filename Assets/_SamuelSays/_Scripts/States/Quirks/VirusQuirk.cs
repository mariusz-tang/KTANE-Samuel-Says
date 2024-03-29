using System;
using System.Collections;
using UnityEngine;

using Rnd = UnityEngine.Random;

public class VirusQuirk : State {

    private readonly string[] _hackedFaces = new string[] {
        ">:(",
        ">:[",
        ">:<",
        ":'(",
        ">:x",
        ":|",
        ">:|",
        ":s",
        ":o",
        ":0",
        ":O"
    };

    private string _inputtedSequence;
    private string _expectedSequence;

    private bool _isFlashingFace = true;
    private bool _isTransitioning = true;

    private Coroutine _flashFace;

    public VirusQuirk(SamuelSaysModule module) : base(module) { }

    public override IEnumerator OnStateEnter() {
        _inputtedSequence = string.Empty;
        _module.Screen.StopSequence();
        _module.StartCoroutine(OnStateEnterAnimation());

        switch (_module.StageNumber) {
            case 2: _expectedSequence = "1234"; break;
            case 3: _expectedSequence = "3412"; break;
            case 4: _expectedSequence = "2413"; break;
            default: throw new ArgumentOutOfRangeException("WTF STAGE NUMBER ARE WE ON :(");
        }
        _module.LogQuirk("Virus");
        _module.Log("There have been " + (_module.StageNumber - 1) + " Submissions so far. Expecting sequence: " + _expectedSequence
             + " (reading order).");

        yield return null;
    }

    public override IEnumerator HandlePress(ColouredButton button) {
        if (_isTransitioning) {
            yield break;
        }

        button.PlayPressAnimation();
        _inputtedSequence += (int)button.Colour + 1;
        yield return null;
    }

    public override IEnumerator HandleRelease(ColouredButton button) {
        button.PlayReleaseAnimation();

        if (_inputtedSequence[_inputtedSequence.Length - 1] != _expectedSequence[_inputtedSequence.Length - 1]) {
            _module.Strike("Incorrectly pressed " + _inputtedSequence + "! Input has been reset.", "Virus Strike");
            _inputtedSequence = string.Empty;

            if (_flashFace != null) {
                _module.StopCoroutine(_flashFace);
            }

            _module.SymbolDisplay.DisplayLetter((_module.StageNumber - 1).ToString());
            yield return new WaitForSeconds(1);
            _flashFace = _module.StartCoroutine(FlashFace());
        }
        else if (_inputtedSequence == _expectedSequence) {
            _module.Log("Pressed the correct sequence!");
            _isFlashingFace = false;
            _module.StartCoroutine(OnStateExitAnimation());
        }
        yield return null;
    }

    private IEnumerator OnStateEnterAnimation() {
        float elapsedTime = 0;
        float transitionTime = 1;
        int flashCount = 4;

        _module.Audio.PlaySoundAtTransform("VirusQuirk Transition", _module.transform);
        _isTransitioning = true;

        for (int i = 0; i < flashCount; i++) {
            foreach (ColouredButton button in _module.Buttons) {
                button.SetVirusColourActive();
            }
            _module.SymbolDisplay.DisplayEmoticon(_hackedFaces[Rnd.Range(0, _hackedFaces.Length)], Color.magenta);

            while (elapsedTime < (2 * i + 1f) / (2f * flashCount) * transitionTime) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            foreach (ColouredButton button in _module.Buttons) {
                button.SetVirusColourInactive();
            }
            _module.SymbolDisplay.DisplayEmoticon(_module.HappyFaces[Rnd.Range(0, _module.HappyFaces.Length)], Color.green);

            while (elapsedTime < (i + 1f) / flashCount * transitionTime) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        foreach (ColouredButton button in _module.Buttons) {
            button.SetVirusColourActive();
        }
        _module.SymbolDisplay.DisplayEmoticon(_hackedFaces[Rnd.Range(0, _hackedFaces.Length)], Color.magenta);
        _flashFace = _module.StartCoroutine(FlashFace());
        _isTransitioning = false;
    }

    private IEnumerator FlashFace() {
        while (_isFlashingFace) {
            _module.SymbolDisplay.DisplayEmoticon(_hackedFaces[Rnd.Range(0, _hackedFaces.Length)], Color.magenta);
            yield return new WaitForSeconds(0.8f);
            _module.SymbolDisplay.ClearScreen();
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }

    private IEnumerator OnStateExitAnimation() {
        float elapsedTime = 0;
        float transitionTime = 1;
        int flashCount = 4;

        _module.Audio.PlaySoundAtTransform("VirusQuirk Transition", _module.transform);
        _isTransitioning = true;

        for (int i = 0; i < flashCount; i++) {
            foreach (ColouredButton button in _module.Buttons) {
                button.SetVirusColourInactive();
            }
            _module.SymbolDisplay.DisplayEmoticon(_module.HappyFaces[Rnd.Range(0, _module.HappyFaces.Length)], Color.green);

            while (elapsedTime < (2 * i + 1f) / (2f * flashCount) * transitionTime) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            foreach (ColouredButton button in _module.Buttons) {
                button.SetVirusColourActive();
            }
            _module.SymbolDisplay.DisplayEmoticon(_hackedFaces[Rnd.Range(0, _hackedFaces.Length)], Color.magenta);

            while (elapsedTime < (i + 1f) / flashCount * transitionTime) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        foreach (ColouredButton button in _module.Buttons) {
            button.SetVirusColourInactive();
        }
        _module.SymbolDisplay.DisplayEmoticon(_module.HappyFaces[Rnd.Range(0, _module.HappyFaces.Length)], Color.green);
        yield return new WaitForSeconds(1);
        _module.SymbolDisplay.ClearScreen();
        _module.ChangeState(new RegularStage(_module));
        _isTransitioning = false;
    }

    public override TpAction NextTpAction() {
        if (_isTransitioning) {
            return new TpAction(TpActionType.Wait);
        }

        int position = _inputtedSequence.Length;

        return new TpAction(TpActionType.PressShort, _expectedSequence[position] - '1');
    }
}
