using Spright.Web.Models.Requests;
using Spright.Web.Models.Responses;
using Spright.Web.Services;
using Spright.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Spright.Web.Controllers.Api
{
    [RoutePrefix("api/Followers")]
    public class FollowerApiController : ApiController
    {
        private IFollowersServices _FollowerService { get; set; } 

        public FollowerApiController(IFollowersServices followerService)
        {
            _FollowerService = followerService;
        }



        [Route("Post"), HttpPost]
            public HttpResponseMessage FollowUser(FollowersRequestModel model)
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

            ItemResponse<int> response = new ItemResponse<int>();

                response.Item = _FollowerService.InsertFollow(model);
                return Request.CreateResponse(response);
            }

        // list my followers
        [Route("{UserId:Guid}"), HttpGet]
        public HttpResponseMessage GetMyFollowers(Guid UserId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemsResponse<Domain.UserDetails> response = new ItemsResponse<Domain.UserDetails>();
            response.Items = _FollowerService.GetUserById(UserId, "follower");
            return Request.CreateResponse(response);
        }

        // list my followings
        [Route("following/{UserId:Guid}"), HttpGet]
        public HttpResponseMessage GetMyFollowings(Guid UserId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemsResponse<Domain.UserDetails> response = new ItemsResponse<Domain.UserDetails>();
            response.Items = _FollowerService.GetUserById(UserId, "following");
            return Request.CreateResponse(response);
        }

        //Unfollow
        [Route("{FollowedId:guid}"), HttpDelete]
        public HttpResponseMessage UnFollow(Guid FollowedId)
        {
            SuccessResponse response = new SuccessResponse();

            _FollowerService.UnFollow(FollowedId);

            return Request.CreateResponse(response);
        }

        }
}
