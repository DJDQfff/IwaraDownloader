﻿using System;
using System.Linq;

using IwaraDatabase.Entities;

using Microsoft.EntityFrameworkCore;

namespace IwaraDatabase.Operation
{
    public static class Tools
    {
        /// <summary>
        ///合并相同项之后添加到数据库
        /// </summary>
        public static void AddWithoutRepeat (this MonthInfo newmonth)
        {
            Database database = new Database();
            var month = database.MonthInfos
                .Include(n => n.MMDs)
                 .Where(n => n.Month == newmonth.Month)
                 .Where(n => n.Year == newmonth.Year)
                 .SingleOrDefault();

            if (month != null)
            {
                var list = month.MMDs;
                var newmmdlist = newmonth.MMDs;
                foreach (MMDInfo newmmd in newmmdlist)
                {
                    var repeat = list.Where(n => n.Hash == newmmd.Hash).SingleOrDefault();
                    if (repeat == null)
                    {
                        list.Add(newmmd);
                    }
                    else
                    {
                        list.Remove(repeat);
                        list.Add(newmmd);
                    }
                }
                database.SaveChanges();
            }
            return;
        }

        /// <summary>
        /// 切换这个MMD下载状态
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="trueorfalse"></param>
        public static void SetWhetherDownloaded (this string hash, bool trueorfalse)
        {
            Database database = new Database();
            var mmd = database.MMDInfos
                .Where(n => n.Hash == hash)
                .Single();
            mmd.WhetherDownloaded = trueorfalse;
            database.SaveChanges();
        }

        /// <summary> 寻找某MMD的名称</summary>
        /// <param name="hash"> mmd的hash </param>
        /// <returns> mmd的title </returns>
        /// <exception cref="InvalidOperationException">
        /// 不止一个元素
        /// </exception>
        public static string GetTitle (this string hash)
        {
            Database database = new Database();
            var titel = database.MMDInfos
                .Where(n => n.Hash == hash)
                .Select(n => n.Title)
                .Single();
            return titel;
        }
    }
}