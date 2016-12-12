using System;
using System.Linq;
using BizMall.Data.DBContexts;
using BizMall.Data.Repositories.Abstract;
using BizMall.Models.CommonModels;
using BizMall.Models.CompanyModels;
using Microsoft.EntityFrameworkCore;

namespace BizMall.Data.Repositories.Concrete
{
    public class RepositoryImage : RepositoryBase, IRepository, IRepositoryImage  
    {
        public RepositoryImage(ApplicationDbContext ctx) : base(ctx)
        {
           
        }

        public Image SaveImage(Image item)
        {
            _ctx.Images.Add(item);
            _ctx.SaveChanges();
            return item; 
        }

        public void ChangeImageToDeleteStatus(int imageId)
        {
            Image dbEntry = _ctx.Images.Where(i => i.Id == imageId).FirstOrDefault();
            dbEntry.ToDelete = true;

            _ctx.Entry(dbEntry).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public void ChangeImagesToNonDeleteStatus(int[] ids)
        {
            var images = _ctx.Images.Where(i => ids.Contains(i.Id));
            foreach(var im in images)
                im.ToDelete = false;

            _ctx.Images.UpdateRange(images);
            _ctx.SaveChanges();
        }

        public void DeleteImage(int imageId) {
            Image image = _ctx.Images.Where(i => i.Id == imageId).FirstOrDefault();

            _ctx.Images.Remove(image);
            _ctx.SaveChanges();
        }

        public void DeleteAllGoodImages(int goodId) {
            Article dbEntry = _ctx.Articles.Where(g => g.Id == goodId)
                       .Include(g => g.Category)
                       .Include(g => g.Category.ParentCategory)
                       .Include(g => g.Images)
                       .SingleOrDefault();

            dbEntry.Images.Clear();
            _ctx.Entry(dbEntry).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public Image GetImage(int ImageId) {
            return _ctx.Images.Where(i => i.Id == ImageId).SingleOrDefault();
        }

        public Image GetGoodImage(int GoodId)
        {
            //return _ctx.Images.Where(i => i.GoodId == GoodId).SingleOrDefault();
            return null;
        }

        public void DeleteImages(int[] ids)
        {
            var images = _ctx.Images.Where(i => ids.Contains(i.Id));
            _ctx.Images.RemoveRange(images);
            _ctx.SaveChanges();
        }
    }
}
