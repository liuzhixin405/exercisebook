using Dapper;
using DocumentFormat.OpenXml.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Common.Requests
{
    public class AdminCPRequest
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        public QitemCollection q { get; set; } = new QitemCollection();
        /// <summary>
        /// 分页
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 分页大小
        /// </summary>
        public int Limit { get; set; } = 20;
        public List<Sort> Sort { get; set; } = new List<Sort>();
        ///// <summary>
        ///// 获取查询条件
        ///// </summary>
        //public List<Qitem> GetQitems()
        //{
        //    List<Qitem> list = new List<Qitem>();
        //    foreach (var item in q)
        //    {
        //        list.Add(new Qitem() {
        //          Name=item.Key
        //        });
        //    }
        //    return list;
        //}
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    public class Qitem
    {
        private string _name;
        /// <summary>
        /// 表单名
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value.Contains("'") || value.Contains(" "))
                {
                    throw new NotSupportedException("error format value");
                }
                _name = value;
            }
        }
        /// <summary>
        /// 位置号
        /// </summary>
        public int Index { get; set; }
        private string _fieldName;
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName
        {
            get
            {
                if (_fieldName != null)
                {
                    return _fieldName;
                }
                else
                {
                    return _name;
                }
            }
            set
            {
                if (value.Contains("'") || value.Contains(" "))
                {
                    throw new NotSupportedException("error format value");
                }
                _fieldName = value.Replace("'", "");
            }
        }
        private string _op = "and";
        /// <summary>
        /// 操作符
        /// </summary>
        public string Op { get => _op; set => _op = value == "or" ? "and" : "or"; }
        /// <summary>
        /// 查询值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 查询值类型
        /// </summary>
        public QValueType ValueType { get; set; }
        /// <summary>
        /// 允许空值
        /// </summary>
        public bool IsAllowEmptyValue { get; set; }
        /// <summary>
        /// 条件类型
        /// </summary>
        public QitemType Type { get; set; } = QitemType.Eq;
        /// <summary>
        /// 重载
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Value != null && (Convert.ToString(Value) != string.Empty || IsAllowEmptyValue))
            {
                if (ValueType == QValueType.Source)
                {
                    stringBuilder.Append($" {Value} ");
                }
                else if (ValueType == QValueType.Array)
                {
                    stringBuilder.Append($" {Op} {FieldName} in @{Name}{Index}");
                }
                else
                {
                    if (Type == QitemType.Like)
                    {
                        stringBuilder.Append($" {Op} {FieldName} like concat('%',@{Name}{Index},'%') ");
                    }
                    else if (Type == QitemType.Between)
                    {
                        stringBuilder.Append($" {Op} {FieldName} Between @{Name}{Index}1 and @{Name}{Index}2 ");
                    }
                    else
                    {
                        stringBuilder.Append($" {Op} {FieldName}{Type.ToString()}@{Name}{Index} ");
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
    /// <summary>
    /// 条件类型
    /// </summary>
    public class QitemType
    {
        /// <summary>
        /// 不相等
        /// </summary>
        public const string Ne = "!=";
        /// <summary>
        /// 相等
        /// </summary>
        public const string Eq = "=";
        /// <summary>
        /// 相似
        /// </summary>
        public const string Like = "like";
        /// <summary>
        /// 小于等于
        /// </summary>
        public const string Le = "<=";
        /// <summary>
        /// 小于
        /// </summary>
        public const string Lt = "<";
        /// <summary>
        /// 大于
        /// </summary>
        public const string Gt = ">";
        /// <summary>
        /// 大于等于
        /// </summary>
        public const string Ge = ">=";
        /// <summary>
        /// Between
        /// </summary>
        public const string Between = "between";

        public string Value { get; set; } = QitemType.Eq;
        public static implicit operator QitemType(string v)
        {
            switch (v)
            {
                case QitemType.Ne:
                    return new QitemType() { Value = QitemType.Ne };
                case QitemType.Like:
                    return new QitemType() { Value = QitemType.Like };
                case QitemType.Le:
                    return new QitemType() { Value = QitemType.Le };
                case QitemType.Lt:
                    return new QitemType() { Value = QitemType.Lt };
                case QitemType.Gt:
                    return new QitemType() { Value = QitemType.Gt };
                case QitemType.Ge:
                    return new QitemType() { Value = QitemType.Ge };
                case QitemType.Between:
                    return new QitemType() { Value = QitemType.Between };
                case Eq:
                case "":
                case null:
                    return new QitemType() { Value = Eq };
                default:
                    throw new NotSupportedException(v);
            }
        }
        public static bool operator ==(QitemType qType, string v)
        {
            return qType.ToString() == v;
        }
        public static bool operator !=(QitemType qType, string v)
        {
            return qType.ToString() != v;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    /// <summary>
    /// 值类型
    /// </summary>
    public enum QValueType
    {
        /// <summary>
        /// 默认传参
        /// </summary>
        None,
        /// <summary>
        /// 相差天数
        /// </summary>
        AddDay,
        /// <summary>
        /// 源数据
        /// </summary>
        Source,
        /// <summary>
        /// 数组
        /// </summary>
        Array
    }
    /// <summary>
    /// 排序
    /// </summary>
    public class Sort
    {
        private string _name;
        /// <summary>
        /// 排序字段名称
        /// </summary>
        public string Name { get { return _name; } set { _name = value.Replace("'", ""); } }

        private string _order = "asc";
        /// <summary>
        /// 排序方向，asc：升序,desc:倒序
        /// </summary>
        public string Order { get { return _order; } set { _order = value.Replace("'", ""); } }
    }
    public static class QitemExtension
    {
        /// <summary>
        /// 转成mysql查询参数
        /// </summary>
        /// <param name="qitems"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToParam(this QitemCollection qitems)
        {
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            foreach (var item in qitems)
            {
                if (item.Type == QitemType.Between)
                {
                    var vs = (Newtonsoft.Json.Linq.JArray)item.Value;
                    pairs.Add($"{item.Name}{item.Index}1", vs[0].ToString());
                    pairs.Add($"{item.Name}{item.Index}2", vs[1].ToString());
                }
                else
                {
                    pairs.Add($"{item.Name}{item.Index}", item.Value);
                }
            }
            return pairs;
        }

        /// <summary>
        /// 转成mysql查询参数
        /// </summary>
        /// <param name="qitems"></param>
        /// <returns></returns>
        public static DynamicParameters ToParameters(this QitemCollection qitems)
        {
            var pairs = new DynamicParameters();
            foreach (var item in qitems)
            {
                if (item.Type == QitemType.Between)
                {
                    var vs = (Newtonsoft.Json.Linq.JArray)item.Value;
                    pairs.Add($"@{item.Name}{item.Index}1", vs[0].ToString());
                    pairs.Add($"@{item.Name}{item.Index}2", vs[1].ToString());
                }
                else
                {
                    pairs.Add($"@{item.Name}{item.Index}", item.Value);
                }
            }
            return pairs;
        }
    }
    public class QitemCollection : IList<Qitem>
    {
        private List<Qitem> _qitems = new List<Qitem>();
        public int Count => _qitems.Count;

        public bool IsReadOnly => false;
        private int CurIndex = 0;

        public Qitem this[int index] { get => _qitems[index]; set => _qitems[index] = value; }

        public void Add(Qitem item)
        {
            CurIndex++;
            item.Index = CurIndex;
            _qitems.Add(item);
        }

        public void Clear()
        {
            _qitems.Clear();
        }

        public bool Contains(Qitem item)
        {
            return _qitems.Contains(item);
        }

        public void CopyTo(Qitem[] array, int arrayIndex)
        {
            _qitems.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Qitem> GetEnumerator()
        {
            return _qitems.GetEnumerator();
        }

        public bool Remove(Qitem item)
        {
            return _qitems.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _qitems.GetEnumerator();
        }
        public override string ToString()
        {
            return base.ToString();
        }

        public int IndexOf(Qitem item)
        {
            return _qitems.IndexOf(item);
        }

        public void Insert(int index, Qitem item)
        {
            _qitems.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _qitems.RemoveAt(index);
        }
    }

}
