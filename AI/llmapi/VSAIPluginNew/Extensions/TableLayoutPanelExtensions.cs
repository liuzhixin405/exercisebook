using System;
using System.Windows.Forms;

namespace VSAIPluginNew.Extensions
{
    /// <summary>
    /// TableLayoutPanel的扩展方法类
    /// </summary>
    public static class TableLayoutPanelExtensions
    {
        /// <summary>
        /// 设置TableLayoutPanel的行可见性
        /// </summary>
        /// <param name="panel">要操作的面板</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="visible">是否可见</param>
        public static void SetRowVisible(this TableLayoutPanel panel, int rowIndex, bool visible)
        {
            if (rowIndex < 0 || rowIndex >= panel.RowCount)
                return;

            if (visible)
                panel.RowStyles[rowIndex] = new RowStyle(SizeType.Absolute, 150);
            else
                panel.RowStyles[rowIndex] = new RowStyle(SizeType.Absolute, 0);
        }
    }
} 