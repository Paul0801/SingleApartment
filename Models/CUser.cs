﻿using Newtonsoft.Json;
using sln_SingleApartment.ViewModel;
using sln_SingleApartment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace sln_SingleApartment.Models
{
    public class CUser
    {
        private SingleApartmentEntities db = new SingleApartmentEntities();
        public tMember tMember { get; set; }
        
        #region 商城組

        #region 我的最愛
        //查詢我的最愛
        public List<CFavoriteList> SearchFavorite()
        {
            List<CFavoriteList> list = new List<CFavoriteList>();
            var fav = db.FavoriteList.Where(r => r.MemberID == tMember.fMemberId);
            foreach (var item in fav)
            {
                CFavoriteList Memberfav = new CFavoriteList() { entity = item };
                list.Add(Memberfav);
            }
            return list;
        }
        //將產品加到我的最愛
        public string AddToFavorite(int ProductID)
        {
            var pdName = db.Product.Where(r => r.ProductID == ProductID).FirstOrDefault().ProductName;
            var fa = db.FavoriteList.Where(r => r.MemberID == tMember.fMemberId && r.ProductID == ProductID);
            if (fa.Count() == 0)
            {
                try
                {
                    FavoriteList fv = new FavoriteList() { MemberID = tMember.fMemberId, ProductID = ProductID };
                    db.FavoriteList.Add(fv);
                    db.SaveChanges();
                    return $"已成功將 {pdName} 加入我的最愛";
                }
                catch (Exception)
                {
                    return $"出現錯誤！請稍後再嘗試！";
                }
            }
            else return "我的最愛裡已有此件商品";

        }
        //刪除我的最愛
        public string DeleteFavorite(int ProductID)
        {
            var fa = db.FavoriteList.Where(r => r.MemberID == tMember.fMemberId && r.ProductID == ProductID).FirstOrDefault();
            if (fa != null)
            {
                db.FavoriteList.Remove(fa);
                db.SaveChanges();
                return "刪除成功";
            }
            return "我的最愛裡沒有此商品，請再試一次！";
        }
        #endregion
        #region 購物車
        //查看購物車裡的商品
        public List<COrderDetailsViewModel> SearchProductInCart(List<CAddtoSessionView> list)
        {
            List<COrderDetailsViewModel> orderDetails = new List<COrderDetailsViewModel>();
            
            foreach (var item in list)
            {
                COrderDetailsViewModel odd = new COrderDetailsViewModel();
                odd.entity = new OrderDetails() {
                    ProductID = item.txtProductID,
                    Quantity = item.txtQuantity,
                    ProductName = db.Product.Where(r => r.ProductID == item.txtProductID).FirstOrDefault().ProductName,
                    UnitPrice = db.Product.Where(r => r.ProductID == item.txtProductID).FirstOrDefault().UnitPrice
                };
                orderDetails.Add(odd);
            }
            return orderDetails;
        }
        #endregion
        #region 商品
        //查詢商品
        public ShopViewModel SearchProduct()
        {
            List<CProductViewModel> list_product = new List<CProductViewModel>();
            foreach (var item in db.Product.Where(r => r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID==null))
            {
                list_product.Add(new CProductViewModel() { entity = item });
            }

            List<CProductMainCategoryViewModel> list_main = new List<CProductMainCategoryViewModel>();
            foreach (var item in db.ProductMainCategory)
            {
                list_main.Add(new CProductMainCategoryViewModel()
                {
                    entity_MainCategory = item,
                    ProductCount = list_product.Where(r => r.MainCategoryID == item.ProductMainCategoryID).Count()
                });
            }
            List<CProductSubCategoryViewModel> list_sub = new List<CProductSubCategoryViewModel>();
            foreach (var item in db.ProductSubCategory)
            {
                list_sub.Add(new CProductSubCategoryViewModel()
                {
                    entity_SubCategory = item,
                    ProductCount = list_product.Where(r => r.SubCategoryID == item.ProductSubCategoryID).Count()
                });
            }
            
            ShopViewModel result = new ShopViewModel() { product = list_product, MainCategory = list_main, SubCategory = list_sub };
            return result;
        }
        public List<CProductViewModel> SearchProductsBy(int? MainCategory=null, int? SubCategory=null,string KeyWord="")
        {
            string[] kw = null;
            if (KeyWord !="")
            {
                 kw = KeyWord.Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries);
            }
            List<CProductViewModel> result = new List<CProductViewModel>();
            var pd = db.Product.Where(r => r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID == null);
            if (kw!= null && kw.Length!=0)
            {
                pd = pd.Where(p => kw.Any(x => p.ProductName.Contains(x))).OrderByDescending(pdd=>kw.All(x=>pdd.ProductName.Contains(x)));
            }
            else if (SubCategory!= null)
            {
                pd = db.Product.Where(r => r.ProductSubCategoryID == SubCategory && r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID == null);
            }
            else if (MainCategory!= null)
            {
                pd = db.Product.Where(r => r.ProductSubCategory.ProductMainCategoryID == MainCategory && r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID == null);
            }
            foreach(var item in pd)
            {
                result.Add(new CProductViewModel() { entity = item });
            }
            return result;
        }

        public List<CProductViewModel> SearchProductsByPrice(int FirstPrice, int LastPrice)
        {
            List<CProductViewModel> result = new List<CProductViewModel>();
            var pd = db.Product.Where(r => r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID == null);
            pd = pd.Where(r => r.UnitPrice >= FirstPrice && r.UnitPrice <= LastPrice);
            foreach (var item in pd)
            {
                result.Add(new CProductViewModel() { entity = item });
            }
            return result;
        }
        #endregion
        #region 訂單
        public List<COrder> SearchOrders()
        {
            List<COrder> list = new List<COrder>();
            var orders = db.Order.Where(r => r.MemberID == tMember.fMemberId);
            foreach (var item in orders)
            {
                COrder order = new COrder() { order_entity = item };
                list.Add(order);
            }
            return list;
        }
        public List<COrderDetailsViewModel> SearchOrder(int id)
        {
            List<COrderDetailsViewModel> lt = new List<COrderDetailsViewModel>();
            var odd = db.OrderDetails.Where(r => r.OrderID == id);
            foreach(var item in odd)
            {
                lt.Add(new COrderDetailsViewModel() { entity = item });
            }
            return lt;
        }
        public string DeleteAnOrder(int id)
        {
            Order od = db.Order.FirstOrDefault(p => p.OrderID == id);

            var odd = db.OrderDetails.Where(q => q.OrderID == id);
            if (od != null)
            {
                if (od.OrderDate.AddDays(7) < DateTime.Now)
                    return "此筆訂單已超過七天鑑賞期，無法退貨囉！";
                if (odd.Count() != 0)
                {
                    foreach (var ITEM in odd)
                    {
                        db.OrderDetails.Remove(ITEM);
                    }
                }
                db.Order.Remove(od);
                CInformationFactory x = new CInformationFactory();
                x.Add(tMember.fMemberId, 100, id, 30020);
                db.SaveChanges();
                return "您的訂單已取消～";
            }
            return "發生錯誤，請稍後再試！";
        }
        public string MakeOrder(List<CAddtoSessionView> cart)
        {
            Order order = new Order();
            int totalPrice = 0;
            if (cart!= null && cart.Count() != 0)
            {
                foreach(var item in cart)
                {
                    try
                    {
                        OrderDetails orderDetail = new OrderDetails();
                        orderDetail.ProductID = item.txtProductID;
                        orderDetail.Quantity = item.txtQuantity;
                        //同步修改庫存
                        db.Product.Where(r => r.ProductID == item.txtProductID).FirstOrDefault().Stock -= item.txtQuantity;
                        db.Product.Where(r => r.ProductID == item.txtProductID).FirstOrDefault().Sales += item.txtQuantity;
                        orderDetail.ProductName = db.Product.Where(r => r.ProductID == item.txtProductID).FirstOrDefault().ProductName;
                        orderDetail.UnitPrice = db.Product.Where(r => r.ProductID == item.txtProductID).FirstOrDefault().UnitPrice;
                        totalPrice += item.txtQuantity * orderDetail.UnitPrice;
                        order.OrderDetails.Add(orderDetail);
                    }
                    catch (Exception)
                    {
                        return "發生錯誤，請稍後再試！";
                    }
                }
               
            }
            order.OrderDate = DateTime.Now;
            
            order.TotalAmount = totalPrice;
            order.OrderStatusID = 1;
            order.SendingStatus = "等待付款中";
            order.PayStatus = "尚未付款";
            order.MemberID = this.tMember.fMemberId;
            try
            {
                db.Order.Add(order);
                db.SaveChanges();
                CInformationFactory x = new CInformationFactory();
                x.Add(tMember.fMemberId, 100, order.OrderID, 30010);
                
            }
            catch (Exception)
            {
                return "發生錯誤，請稍後再試！";
            }
            return $"{order.OrderID}";
        }
        #endregion
        #region 智慧辨識
        public class Answer
        {
            public string id { get; set; }
            public string project { get; set; }
            public string iteration { get; set; }
            public string created { get; set; }
            public object[] predictions { get; set; }
        }
        public class MyObject
        {
            public double probability { get; set; }
            public string tagId { get; set; }
            public string tagName { get; set; }
        }
        #endregion
        #endregion

        //將房間加到我的最愛
        public string AddRoomToFavorite(int RoomID)
        {
            var roomName = db.Room.Where(r => r.ID == RoomID).FirstOrDefault().RoomName;
            var roomfa = db.RoomFavorite.Where(r => r.MemberID == tMember.fMemberId && r.RoomID == RoomID);
            if (roomfa.Count() == 0)
            {
                try
                {
                    RoomFavorite roomfv = new RoomFavorite() { MemberID = tMember.fMemberId, RoomID = RoomID };
                    db.RoomFavorite.Add(roomfv);
                    db.SaveChanges();

                    //50010 加入我的最愛
                    CInformationFactory x = new CInformationFactory();
                    x.Add(tMember.fMemberId, 400, 0, 40010);

                    return $"已成功將 {roomName} 加入我的最愛";
                }
                catch (Exception)
                {
                    return $"出現錯誤！請稍後再嘗試！";
                }
            }
            else return "我的最愛裡已有此房間";

        }

        //查詢房間的我的最愛
        public List<CRoomFavorite> SearchRoomFavorite()
        {
            List<CRoomFavorite> list = new List<CRoomFavorite>();
            var romfav = db.RoomFavorite.Where(r => r.MemberID == tMember.fMemberId);
            foreach (var item in romfav)
            {
                CRoomFavorite MemberRoomfav = new CRoomFavorite() { entity_RoomFavorite = item };
                list.Add(MemberRoomfav);
            }
            return list;
        }

        //刪除房間的我的最愛
        public string DeleteRoomFavorite(int RoomID)
        {
            var roomfa = db.RoomFavorite.Where(r => r.MemberID == tMember.fMemberId && r.RoomID == RoomID).FirstOrDefault();
            if (roomfa != null)
            {
                db.RoomFavorite.Remove(roomfa);
                db.SaveChanges();
                return "刪除成功";
            }
            return "我的最愛裡沒有此商品，請再試一次！";
        }


    }
}