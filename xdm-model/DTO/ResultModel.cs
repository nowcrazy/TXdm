﻿using System;
namespace xdm_model.DTO
{
    public class ResultModel<T>
    {
        public ResultModel()
        {
        }
        /// <summary>
        /// 返回结果编码 0：失败 1：成功
        /// </summary>
        public int ResultCode { get; set; }

        /// <summary>
        /// 返回结果内容 成功：Success  失败：异常内容
        /// </summary>
        public string? ResultMsg { get; set; }

        /// <summary>
        /// 返回结果 成功：返回T类型数据 失败：默认null
        /// </summary>
        public T? ResultData { get; set; }
    }
}

