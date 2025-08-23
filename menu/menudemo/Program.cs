using menuemo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace menudemo
{
   

    internal class Program
    {
        private static List<Menu> menus = new List<Menu>();
        static void Main()
        {
            SeedData();
            BuildNavigationPath();
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var category = menus.FirstOrDefault(c => c.Id == id);
                if (category != null)
                {
                    // 打印从父到子路径
                    var path = GetPath(category);
                    Console.WriteLine("路径: " + string.Join(" -> ", path.Select(c => c.Name)));
                }
                else
                {
                    Console.WriteLine("未找到该 Id 对应的分类。");
                }
            }
            else
            {
                Console.WriteLine("输入无效。");
            }
            Console.ReadLine(); 
        }

        static void SeedData()
        {
            var menuDashboard = new Menu { Id = 1, Name = "仪表盘" };
            var menuUser = new Menu { Id = 2, Name = "用户管理" };
            var menuProduct = new Menu { Id = 3, Name = "商品管理" };
            var menuOrder = new Menu { Id = 4, Name = "订单管理" };
            var menuSettings = new Menu { Id = 5, Name = "系统设置" };

            // 子菜单
            var menuUserList = new Menu { Id = 6, Name = "用户列表", ParentId = 2, Parent = menuUser };
            var menuUserRole = new Menu { Id = 7, Name = "角色权限", ParentId = 2, Parent = menuUser };

            var menuProductList = new Menu { Id = 8, Name = "商品列表", ParentId = 3, Parent = menuProduct };
            var menuProductCategory = new Menu { Id = 9, Name = "商品分类", ParentId = 3, Parent = menuProduct };

            var menuOrderList = new Menu { Id = 10, Name = "订单列表", ParentId = 4, Parent = menuOrder };
            var menuOrderReturn = new Menu { Id = 11, Name = "退货管理", ParentId = 4, Parent = menuOrder };

            var menuSettingsGeneral = new Menu { Id = 12, Name = "通用设置", ParentId = 5, Parent = menuSettings };
            var menuSettingsSecurity = new Menu { Id = 13, Name = "安全设置", ParentId = 5, Parent = menuSettings };

            // 建立层级关系
            menuUser.Children.Add(menuUserList);
            menuUser.Children.Add(menuUserRole);

            menuProduct.Children.Add(menuProductList);
            menuProduct.Children.Add(menuProductCategory);

            menuOrder.Children.Add(menuOrderList);
            menuOrder.Children.Add(menuOrderReturn);

            menuSettings.Children.Add(menuSettingsGeneral);
            menuSettings.Children.Add(menuSettingsSecurity);

            // 添加到根集合
            menus.Add(menuDashboard);
            menus.Add(menuUser);
            menus.Add(menuProduct);
            menus.Add(menuOrder);
            menus.Add(menuSettings);
            menus.Add(menuUserList);
            menus.Add(menuUserRole);
            menus.Add(menuProductList);
            menus.Add(menuProductCategory);
            menus.Add(menuOrderList);
            menus.Add(menuOrderReturn);
            menus.Add(menuSettingsGeneral);
            menus.Add(menuSettingsSecurity);
        }

        static void BuildNavigationPath()
        {
            var lookup = menus.ToDictionary(m => m.Id,m=>m);
            foreach(var menu in menus)
            {
                if(menu.ParentId.HasValue && lookup.ContainsKey(menu.ParentId.Value))
                {
                    menu.Parent = lookup[menu.ParentId.Value];
                    lookup[menu.ParentId.Value].Children.Add(menu);
                }
            }
        }
        static List<Menu> GetPath(Menu menu)
        {
           var path = new List<Menu>();
           while (menu != null)
           {
               path.Add(menu);
               menu = menu.Parent;
           }
            return path;
        }
    }

}