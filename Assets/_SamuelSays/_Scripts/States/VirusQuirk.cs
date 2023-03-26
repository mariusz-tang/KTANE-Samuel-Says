using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private readonly string[] _happyFaces = new string[] {
        ":)",
        ": )",
        ":-)",
        "=)",
        "= )",
        "=-)",
        ":]" ,
        ": ]",
        ":-]",
        "=]",
        "= ]",
        "=-]"
    };

    private string _inputtedSequence = string.Empty;
    private string _expectedSequence;

    private bool _isFlashingFace = true;
    private bool _isTransitioning = true;

    public VirusQuirk(SamuelSaysModule module) : base(module) { }

    public override IEnumerator OnStateEnter() {
        _module.Screen.StopSequence();
        _module.StartCoroutine(OnStateEnterAnimation());

        switch (_module.StageNumber) {
            case 2: _expectedSequence = "1234"; break;
            case 3: _expectedSequence = "3412"; break;
            case 4: _expectedSequence = "2413"; break;
            default: throw new ArgumentOutOfRangeException("WTF STAGE NUMBER ARE WE ON :(");
        }
        _module.Log("-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-");
        _module.Log("Quirk: Virus! Expecting sequence: " + _expectedSequence + ".");

        yield return null;
    }

    public override IEnumerator HandlePress(ColouredButton button) {
        if (_isTransitioning) {
            yield break;
        }

        _inputtedSequence += (int)button.Colour + 1;
        if (_inputtedSequence[_inputtedSequence.Length - 1] != _expectedSequence[_inputtedSequence.Length - 1]) {
            _module.Strike("Incorrectly inputted " + _inputtedSequence + "! Strike!");
            _inputtedSequence = string.Empty;
        }
        else if (_inputtedSequence == _expectedSequence) {
            _module.Log("Inputted the correct sequence!");
            _isFlashingFace = false;
            _module.StartCoroutine(OnStateExitAnimation());
        }
        yield return null;
    }

    private IEnumerator OnStateEnterAnimation() {
        float elapsedTime = 0;
        float transitionTime = 1;
        int flashCount = 4;

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
            _module.SymbolDisplay.DisplayEmoticon(_happyFaces[Rnd.Range(0, _happyFaces.Length)], Color.green);

            while (elapsedTime < (i + 1f) / flashCount * transitionTime) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        foreach (ColouredButton button in _module.Buttons) {
            button.SetVirusColourActive();
        }
        _module.SymbolDisplay.DisplayEmoticon(_hackedFaces[Rnd.Range(0, _hackedFaces.Length)], Color.magenta);
        _module.StartCoroutine(FlashFace());
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

        _isTransitioning = true;

        for (int i = 0; i < flashCount; i++) {
            foreach (ColouredButton button in _module.Buttons) {
                button.SetVirusColourInactive();
            }
            _module.SymbolDisplay.DisplayEmoticon(_happyFaces[Rnd.Range(0, _happyFaces.Length)], Color.green);

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
        _module.SymbolDisplay.DisplayEmoticon(_happyFaces[Rnd.Range(0, _happyFaces.Length)], Color.green);
        yield return new WaitForSeconds(1);
        _module.SymbolDisplay.ClearScreen();
        _module.ChangeState(new RegularStage(_module));
        _isTransitioning = false;
    }
}