using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Models.View;
using System;

namespace Rcv.LabelTool.Logic.Factories
{
    /// <summary>
    /// Factory for creation of label-representation objects.
    /// </summary>
    public class LabelFactory
    {
        /// <summary>
        /// Create label object from label view.
        /// </summary>
        /// <param name="labelView">Source label view object</param>
        /// <returns>Label representation of view</returns>
        public static Label Create(LabelView labelView)
        {
            #region validation

            if (labelView == null)
            {
                throw new ArgumentNullException(nameof(labelView));
            }

            #endregion

            return new Label()
            {
                Id = labelView.Id,
                ObjectClassName = labelView.ObjectClassName,
                ObjectClassId = labelView.ObjectClassId,
                Left = labelView.Left,
                Top = labelView.Top,
                Right = labelView.Right,
                Bottom = labelView.Bottom,
                Confidence = labelView.Confidence
            };
        }

        /// <summary>
        /// Create label view object from label.
        /// </summary>
        /// <param name="label">Source label object</param>
        /// <returns>View representation of label</returns>
        public static LabelView CreateView(Label label)
        {
            #region validation

            if (label == null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            #endregion

            return new LabelView()
            {
                Id = label.Id,
                ObjectClassName = label.ObjectClassName,
                ObjectClassId = label.ObjectClassId,
                Left = label.Left,
                Top = label.Top,
                Right = label.Right,
                Bottom = label.Bottom,
                Confidence = label.Confidence
            };
        }
    }
}
