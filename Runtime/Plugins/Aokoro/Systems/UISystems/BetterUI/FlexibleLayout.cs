using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace Aokoro.UI
{
    [AddComponentMenu("Layout/FlexibleLayout")]
    public class FlexibleLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform = 0,
            Width = 1,
            Height = 2,
            FixedColumns = 4,
            FixedRows= 8,
        }
        [BoxGroup("Structure")]
        [SerializeField, EnableIf(nameof(fitType), FitType.FixedRows), Min(0)]
        private int rows;
        [BoxGroup("Structure")]
        [SerializeField, EnableIf(nameof(fitType), FitType.FixedColumns), Min(0)]
        private int columns;
        [BoxGroup("Cells")]
        [SerializeField, ShowIf(EConditionOperator.Or, nameof(fitX), nameof(fitY))]
        private Vector2 cellSize;
        [BoxGroup("Cells")]
        [SerializeField]
        private Vector2 spacing;

        [SerializeField, BoxGroup("Fit")]
        FitType fitType;
        [SerializeField, BoxGroup("Fit")]
        private bool fitX;
        [SerializeField, BoxGroup("Fit")]
        private bool fitY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (fitType == FitType.Uniform || fitType == FitType.Width || fitType == FitType.Height)
            {
                fitX = true;
                fitY = true;

                float sqrRT = Mathf.Sqrt(transform.childCount);
                rows = Mathf.CeilToInt(sqrRT);
                columns = Mathf.CeilToInt(sqrRT);
            }

            if(fitType == FitType.Width || fitType == FitType.FixedColumns)
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);

            if(fitType == FitType.Height || fitType == FitType.FixedRows)
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);

            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float fColumns = columns;
            float fRows = rows;

            float cellWidth = (parentWidth / fColumns) 
                - ((spacing.x / fColumns) * (columns - 1)) 
                - (padding.left / fColumns) - (padding.right / fColumns);
            float cellHeight = parentHeight / fRows 
                - ((spacing.y / fRows) * (rows - 1))
                - (padding.top / fRows) - (padding.bottom / fRows);

            if(fitX)
                cellSize.x = cellWidth;
            
            if(fitY)
                cellSize.y = cellHeight;

            int columnCount;
            int rowCount;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                rowCount = i / columns;
                columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = cellSize.x * columnCount + spacing.x * columnCount + padding.left;
                var yPos = cellSize.y * rowCount + spacing.y * rowCount + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }
        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}
