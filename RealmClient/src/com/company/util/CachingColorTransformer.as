package com.company.util
{
   import flash.display.BitmapData;
   import flash.filters.BitmapFilter;
   import flash.geom.ColorTransform;
   import flash.geom.Point;
   import flash.utils.Dictionary;
   
   public class CachingColorTransformer
   {
      
      private static var bds_:Dictionary = new Dictionary();
      private static var alphas_:Dictionary = new Dictionary();
       
      
      public function CachingColorTransformer()
      {
         super();
      }
      
      public static function transformBitmapData(bitmapData:BitmapData, ct:ColorTransform) : BitmapData
      {
         var newBitmapData:BitmapData = null;
         var dict:Dictionary = bds_[bitmapData];
         if(dict != null)
         {
            newBitmapData = dict[ct];
         }
         else
         {
            dict = new Dictionary();
            bds_[bitmapData] = dict;
         }
         if(newBitmapData == null)
         {
            newBitmapData = bitmapData.clone();
            newBitmapData.colorTransform(newBitmapData.rect,ct);
            dict[ct] = newBitmapData;
         }
         return newBitmapData;
      }
      
      public static function filterBitmapData(bitmapData:BitmapData, filter:BitmapFilter) : BitmapData
      {
         var newBitmapData:BitmapData = null;
         var dict:Dictionary = bds_[bitmapData];
         if(dict != null)
         {
            newBitmapData = dict[filter];
         }
         else
         {
            dict = new Dictionary();
            bds_[bitmapData] = dict;
         }
         if(newBitmapData == null)
         {
            newBitmapData = bitmapData.clone();
            newBitmapData.applyFilter(newBitmapData,newBitmapData.rect,new Point(),filter);
            dict[filter] = newBitmapData;
         }
         return newBitmapData;
      }
      
      public static function alphaBitmapData(bitmapData:BitmapData, alpha:int) : BitmapData
      {
         var ct:ColorTransform = alphas_[alpha];
         if (ct == null) {
            ct = new ColorTransform(1, 1, 1, alpha / 100);
            alphas_[alpha] = ct;
         }
         return transformBitmapData(bitmapData,ct);
      }
      
      public static function clear() : void
      {
         var dict:Dictionary = null;
         var bd:BitmapData = null;
         for each(dict in bds_)
         {
            for each(bd in dict)
            {
               bd.dispose();
            }
         }
         bds_ = new Dictionary();
         alphas_ = new Dictionary();
      }
   }
}
