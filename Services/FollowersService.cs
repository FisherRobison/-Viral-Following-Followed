using Spright.Data;
using Spright.Web.Domain;
using Spright.Web.Models.Requests;
using Spright.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Spright.Web.Services
{
    public class FollowersService : BaseService, IFollowersServices
    {
        private SystemEventService _SystemEventService { get; set; }

        public FollowersService(SystemEventService SystemEventService)
        {
            _SystemEventService = SystemEventService;
        }

        public int InsertFollow(FollowersRequestModel model)
        {
            int uid = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Followers_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@FollowerId", UserService.GetCurrentUserId());
                   paramCollection.AddWithValue("@FollowedId", model.FollowedId);


                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int); 
                   p.Direction = System.Data.ParameterDirection.Output;
                   paramCollection.Add(p); 

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out uid);
               });

            SystemEventRequestModel se = new SystemEventRequestModel();
            se.ActorUserId = UserService.GetCurrentUserId();
            se.EventType = Enums.SystemEventType.Followed;
            se.TargetUserId = model.FollowedId.ToString();
            _SystemEventService.Insert(se);

            return uid;
        } 

       
        public  List<UserDetails> GetUserById(Guid id, string type)
        {
            List<UserDetails> list = new List<UserDetails>();
            Media m = null;
            string proc = null;

            if (type == "follower")
            {
                proc = "dbo.Users_Select_My_Followerings";
            }
            else
            {
                proc = "dbo.Users_Select_My_Followers";
            }

            DataProvider.ExecuteCmd(GetConnection, proc
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@ID", id);

               }, map: (Action<IDataReader, short>)delegate (IDataReader reader, short set)
               {
                   UserDetails p = new UserDetails();
                   m = new Media();
                   int startingIndex = 0; 

                   p.Id = reader.GetSafeString(startingIndex++);
                   p.UserName = reader.GetSafeString(startingIndex++);
                   p.FirstName = reader.GetSafeString(startingIndex++);
                   p.LastName = reader.GetSafeString(startingIndex++);
                   p.PhoneNumber = reader.GetSafeString(startingIndex++);
                   p.Email = reader.GetSafeString(startingIndex++);
                   p.DateAdded = reader.GetSafeDateTime(startingIndex++);
                   p.DateModified = reader.GetSafeDateTime(startingIndex++);
                   p.UserType = reader.GetSafeInt32(startingIndex++);
                   m.ID = reader.GetSafeInt32(startingIndex++);
                   m.MediaType = reader.GetSafeString(startingIndex++);
                   m.Path = reader.GetSafeString(startingIndex++);
                   m.FileName = reader.GetSafeString(startingIndex++);
                   m.FileType = reader.GetSafeString(startingIndex++);
                   m.Title = reader.GetSafeString(startingIndex++);
                   m.Description = reader.GetSafeString(startingIndex++);
                 

                   if (m.ID == 0)
                   {
                       p.Avatar = null;
                   }

                   else
                   {
                       p.Avatar = m;
                  }

                   list.Add(p);

               });
           
            return list;
        }

     
        public  bool IsFollowing(Guid FollowedId)
        {
            bool p = false;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Followers_IsFollowing"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@FollowerId", UserService.GetCurrentUserId());
                   paramCollection.AddWithValue("@FollowedId", FollowedId);


               }, map: delegate (IDataReader reader, short set)
               {
                   int startingIndex = 0;
                   p = reader.GetSafeBool(startingIndex++);
               });

            return p;
        }

       
        public  void UnFollow(Guid FollowedId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Followers_Unfollow"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@FollowerId", UserService.GetCurrentUserId());
                   paramCollection.AddWithValue("@FollowedId", FollowedId);

               }, returnParameters: delegate (SqlParameterCollection param)
               {

               });
        }
    }
}