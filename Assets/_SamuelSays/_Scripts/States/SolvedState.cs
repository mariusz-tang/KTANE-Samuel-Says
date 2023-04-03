using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class SolvedState : State {

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
    private readonly int[][] _buttonPressSequence = new int[][] {
            new int[] { 0, 1, 2, 3 },
            new int[] { 0, 1, 2, 3 },
            new int[] { 0, 1, 2, 3 },
            new int[] { 0, 1 },
            new int[] { 2, 3 },
            new int[] { 0, 1 },
            new int[] { 2, 3 },
            new int[] { 0, 1 },
            new int[] { 2, 3 },
            new int[] { 0, 2 },
            new int[] { 1, 3 },
            new int[] { 0, 2 },
            new int[] { 1, 3 },
            new int[] { 0, 2 },
            new int[] { 1, 3 },
            new int[] { 0 },
            new int[] { 1 },
            new int[] { 2 },
            new int[] { 3 },
        };

    public SolvedState(SamuelSaysModule module) : base(module) { }

    public override IEnumerator OnStateEnter() {
        _module.Log("================== Solved ==================");
        _module.Log("Samuel thanks you for your time.");
        _module.Module.HandlePass();

        _module.StartCoroutine(EnterSolveStateAnimation());
        yield return null;
    }

    private IEnumerator EnterSolveStateAnimation() {


        foreach (int[] pressSet in _buttonPressSequence) {
            foreach (int press in pressSet) {
                _module.Buttons[press].PlayPressAnimation();
            }
            _module.SymbolDisplay.DisplayEmoticon(_happyFaces[Rnd.Range(0, _happyFaces.Length)], Color.green);
            yield return new WaitForSeconds(0.5f);

            foreach (int press in pressSet) {
                _module.Buttons[press].PlayReleaseAnimation();
            }

            _module.SymbolDisplay.ClearScreen();
            yield return new WaitForSeconds(0.1f);
        }

        _module.StartCoroutine(ContinuousSolveAnimation());
    }

    private IEnumerator ContinuousSolveAnimation() {
        foreach (int[] pressSet in _buttonPressSequence) {
            foreach (int press in pressSet) {
                _module.Buttons[press].PlayPressAnimation();
            }
            _module.SymbolDisplay.DisplaySolveSmile();
            yield return new WaitForSeconds(0.8f);

            foreach (int press in pressSet) {
                _module.Buttons[press].PlayReleaseAnimation();
            }

            _module.SymbolDisplay.ClearScreen();
            yield return new WaitForSeconds(0.2f);
        }

        _module.StartCoroutine(ContinuousSolveAnimation());
    }
}
