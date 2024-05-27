using System;
namespace xdm_model.DTO
{
    public class AjaxResult : Dictionary<string, object>
    {
        public const string CODE_TAG = "code";
        public const string MSG_TAG = "msg";
        public const string DATA_TAG = "data";
        // 成功，警告和错误状态码
        public const int SUCCESS = 200;
        public const int WARN = 400;
        public const int ERROR = 500;
        public AjaxResult()
        {

        }
        public AjaxResult(int code, String msg)
        {
            this[CODE_TAG] = code;
            this[MSG_TAG] = msg;
        }
        public AjaxResult(int code, string msg, object data)
       : this(code, msg) // 调用另一个构造函数
        {
            if (data != null)
            {
                this[DATA_TAG] = data;
            }

        }
        // 成功，警告和错误消息的快捷创建方法
        public static AjaxResult Success(string msg = "操作成功", object data = null)
        {
            return new AjaxResult(SUCCESS, msg, data);
        }

        public static AjaxResult Warn(string msg, object data = null)
        {
            return new AjaxResult(WARN, msg, data);
        }

        public static AjaxResult Error(string msg = "操作失败", object data = null)
        {
            return new AjaxResult(ERROR, msg, data);
        }

        public static AjaxResult Error(int code, string msg)
        {
            return new AjaxResult(code, msg, null);
        }

        // 检查当前状态码的快捷方法
        public bool IsSuccess() => this[CODE_TAG].Equals(SUCCESS);
        public bool IsWarn() => this[CODE_TAG].Equals(WARN);
        public bool IsError() => this[CODE_TAG].Equals(ERROR);

        // 方便链式调用的扩展方法
        public new AjaxResult Add(string key, object value)
        {
            base.Add(key, value);
            return this;
        }
    }

}

