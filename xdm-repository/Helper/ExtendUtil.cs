using System;
using xdm_model.DTO;

namespace xdm_repository.Helper
{
    public static class ExtendUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static List<MenuItem> BuildTree(List<MenuItem> items, int parentId = 0)
        {
            return items
                .Where(i => i.parent_id == parentId)
                .Select(i => new MenuItem
                {
                    menu_id = i.menu_id,
                    parent_id = i.parent_id,
                    menu_name = i.menu_name,
                    path = i.path,
                    perms = i.perms,
                    icon = i.icon,
                    order_num = i.order_num,
                    Children = BuildTree(items, i.menu_id)
                })
                .ToList();
        }
    }
}

