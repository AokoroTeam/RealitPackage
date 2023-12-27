using LTX;
using LTX.ControlsVisualizer;
using LTX.ControlsVisualizer.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Realit.Core.Player.Controls
{
    public class RealitControlUI : ControlUI
    {
        [SerializeField]
        float widthBorder;
        [SerializeField]
        float heightBorder;
        [SerializeField]
        AnimationCurve alphaCurve;
        [SerializeField]
        float animationTotalTime;

        int shownCommand;
        Timer timer;

        LayoutElement layoutElement;

        List<CanvasGroup> commandsCanvasGroups = new List<CanvasGroup>();

        private void Awake()
        {
            layoutElement = commandsParent.GetComponent<LayoutElement>();
            timer = new Timer(animationTotalTime, false);
            timer.OnTimerEnd += () =>
            {
                if (shownCommand >= commandsCanvasGroups.Count - 1)
                    shownCommand = 0;
                else
                    shownCommand++;
            };

            shownCommand = 0;
        }

        protected override void OnFilledWithCommands()
        {
            float minWidth = 0;
            float minHeight = 0;
            
            foreach (var command in commands)
            {
                if(command.TryGetComponent(out CanvasGroup canvasGroup))
                    commandsCanvasGroups.Add(canvasGroup);

                RectTransform commandRect = command.GetComponent<RectTransform>();

                Rect rect = commandRect.rect;

                minWidth = minWidth < rect.width ? rect.width : minWidth;
                minHeight = minHeight < rect.height ? rect.height : minHeight;
            }

            layoutElement.minWidth = minWidth;
            layoutElement.minHeight = minHeight;

            layoutElement.preferredWidth = minWidth + widthBorder;
            layoutElement.preferredHeight = minHeight + heightBorder;

            foreach (var cg in commandsCanvasGroups)
                cg.alpha = 0;

            commandsCanvasGroups[0].alpha = 1;
        }

        private void Update()
        {
            if(commandsCanvasGroups != null && commandsCanvasGroups.Count != 0)
            {
                if(commandsCanvasGroups.Count > 1)
                {
                    float nTime = timer.NormalizedTime;

                    foreach (var cg in commandsCanvasGroups)
                    {
                        int idx = commandsCanvasGroups.IndexOf(cg);
                        if (idx == shownCommand)
                            cg.alpha = alphaCurve.Evaluate(nTime);
                        else
                            cg.alpha = 0;
                    }
                }
                else
                {
                    commandsCanvasGroups[0].alpha = 1;
                }
            }
        }
    }
}
