using AngularJSAuthentication.API.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class LikeVisitorController : ApiController
    {
        ILikeVisitorRepository _likeVisitorRepo = RepositoryFactory.Create<ILikeVisitorRepository>(ContextTypes.EntityFramework);

        //[HttpGet]
        //[Route("api/likevisitor/get/{clientId}/{hfpdocId}")]
        //[AllowAnonymous]
        //public IHttpActionResult getLikeVisitor(string clientId, string hfpdocId)
        //{
        //    LikeVisitor _likeVisitor = _likeVisitorRepo.Find(x => x.NoorId.Trim() == clientId.Trim() && x.HFP_DOC_NCID.Trim() == hfpdocId.Trim() && x.IsDelete == true).FirstOrDefault();

        //    return Ok(_likeVisitor);
        //}
        [HttpGet]
        [Route("api/likevisitor/get/{hfpdocId}/{clientId?}")]
        [AllowAnonymous]
        public IHttpActionResult getLikeVisitor(string hfpdocId, string clientId = "0")
        {
            int _likecount = _likeVisitorRepo.GetAll().Where(x => x.HFP_DOC_NCID.Trim() == hfpdocId.Trim() && x.IsDelete == false && x.Like_Dislike == true).Count();
            var _visitor = _likeVisitorRepo.GetAll().Where(x => x.HFP_DOC_NCID.Trim() == hfpdocId.Trim() && x.IsDelete == false).ToList();
            bool _isLike = false;
            if (clientId != "0")
            {
                var likes = _likeVisitorRepo.Find(x => x.NoorId == clientId.Trim() && x.HFP_DOC_NCID.Trim() == hfpdocId.Trim() && x.IsDelete == false).FirstOrDefault();
                _isLike = likes != null ? likes.Like_Dislike : false;
            }
            int visitorCount = 0;
            foreach (LikeVisitor element in _visitor)
            {
                visitorCount = visitorCount + element.Visitor;
            }

            var count = new { LikeCount = _likecount, VisitorCount = visitorCount, isLike = _isLike };
            return Ok(count);
        }

        [HttpPost]
        [Route("api/visitor/save/{clientId}/{hfpdocId}")]
        [AllowAnonymous]
        public IHttpActionResult saveLikeVisitor(string clientId, string hfpdocId)
        {
            if (!String.IsNullOrWhiteSpace(clientId) && !String.IsNullOrWhiteSpace(hfpdocId))
            {
                LikeVisitor _likeVisitor = _likeVisitorRepo.Find(x => x.NoorId == clientId.Trim() && x.HFP_DOC_NCID == hfpdocId.Trim() && x.IsDelete == false).FirstOrDefault();
                if (_likeVisitor != null)
                {
                    //update                   
                    _likeVisitor.Visitor = _likeVisitor.Visitor + 1;
                    _likeVisitor.ModifyBy = clientId;
                    _likeVisitor.ModifyDate = DateTime.Now;
                    var result = _likeVisitorRepo.Update(_likeVisitor);
                    return Ok(result);

                }
                else
                {
                    //insert 
                    LikeVisitor _Visitor = new LikeVisitor();
                    _Visitor.CreateDate = DateTime.Now;
                    _Visitor.ModifyDate = DateTime.Now;
                    _Visitor.NoorId = clientId.Trim();
                    _Visitor.HFP_DOC_NCID = hfpdocId.Trim();
                    _Visitor.Visitor = 1;
                    _Visitor.CreatedBy = clientId.Trim();
                    var _likeVisitorCreated = _likeVisitorRepo.Insert(_Visitor);
                    return Ok(_likeVisitorCreated);
                }
            }

            return InternalServerError();
        }

        [HttpPost]
        [Route("api/like/save/{clientId}/{hfpdocId}/{value}")]
        [AllowAnonymous]
        public IHttpActionResult saveLikeVisitor(string clientId, string hfpdocId, bool value)
        {
            if (!String.IsNullOrWhiteSpace(clientId) && !String.IsNullOrWhiteSpace(hfpdocId))
            {
                LikeVisitor _likeVisitor = _likeVisitorRepo.Find(x => x.NoorId == clientId.Trim() && x.HFP_DOC_NCID == hfpdocId.Trim() && x.IsDelete == false).FirstOrDefault();
                if (_likeVisitor != null)
                {
                    //update                   
                    _likeVisitor.Like_Dislike = value;
                    _likeVisitor.ModifyBy = clientId;
                    _likeVisitor.ModifyDate = DateTime.Now;
                    var result = _likeVisitorRepo.Update(_likeVisitor);
                    return Ok(result);

                }
                else
                {
                    //insert 
                    LikeVisitor _Visitor = new LikeVisitor();
                    _Visitor.CreateDate = DateTime.Now;
                    _Visitor.ModifyDate = DateTime.Now;
                    _Visitor.NoorId = clientId.Trim();
                    _Visitor.HFP_DOC_NCID = hfpdocId.Trim();
                    _Visitor.Like_Dislike = value;
                    _Visitor.CreatedBy = clientId.Trim();
                    var _likeVisitorCreated = _likeVisitorRepo.Insert(_Visitor);
                    return Ok(_likeVisitorCreated);
                }
            }

            return InternalServerError();
        }

        //[HttpPost]
        //[Route("api/likevisitor/save")]
        //[AllowAnonymous]
        //public IHttpActionResult saveLikeVisitor([FromBody] LikeVisitor likeVisitor)
        //{
        //    if (likeVisitor != null)
        //    {
        //        LikeVisitor _likeVisitor = _likeVisitorRepo.Find(x => x.NoorId == likeVisitor.NoorId && x.HFP_DOC_NCID == likeVisitor.HFP_DOC_NCID && x.IsDelete == false).FirstOrDefault();
        //        if (_likeVisitor != null)
        //        {
        //            //update
        //            _likeVisitor.Like_Dislike = likeVisitor.Like_Dislike;
        //            _likeVisitor.Visitor = likeVisitor.Visitor;
        //            _likeVisitor.ModifyBy = likeVisitor.ModifyBy;
        //            _likeVisitor.ModifyDate = DateTime.Now;
        //            _likeVisitor.IsDelete = likeVisitor.IsDelete;
        //            var result = _likeVisitorRepo.Update(_likeVisitor);
        //            return Ok(result);

        //        }
        //        else
        //        {
        //            //insert 
        //            likeVisitor.CreateDate = DateTime.Now;
        //            likeVisitor.ModifyDate = DateTime.Now;
        //            var _likeVisitorCreated = _likeVisitorRepo.Insert(likeVisitor);
        //            return Ok(_likeVisitorCreated);
        //        }
        //    }

        //    return InternalServerError();
        //}

    }
}
