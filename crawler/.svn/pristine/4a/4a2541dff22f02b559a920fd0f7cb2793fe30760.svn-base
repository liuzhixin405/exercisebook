using System;
using System.Collections.Generic;
using System.Text;

namespace CrawlerConsole.Model
{
    public class ShortCode
    {

        public class Rootobject
        {
            public Graphql graphql { get; set; }
        }

        public class Graphql
        {
            public Shortcode_Media shortcode_media { get; set; }
        }

        public class Shortcode_Media
        {
            public string __typename { get; set; }
            public string id { get; set; }
            public string shortcode { get; set; }
            public Dimensions dimensions { get; set; }
            public object gating_info { get; set; }
            public object fact_check_overall_rating { get; set; }
            public object fact_check_information { get; set; }
            public object sensitivity_friction_info { get; set; }
            public object media_overlay_info { get; set; }
            public string media_preview { get; set; }
            public string display_url { get; set; }
            public Display_Resources[] display_resources { get; set; }
            public string accessibility_caption { get; set; }
            public bool is_video { get; set; }
            public string tracking_token { get; set; }
            public Edge_Media_To_Tagged_User edge_media_to_tagged_user { get; set; }
            public Edge_Media_To_Caption edge_media_to_caption { get; set; }
            public bool caption_is_edited { get; set; }
            public bool has_ranked_comments { get; set; }
            public Edge_Media_To_Parent_Comment edge_media_to_parent_comment { get; set; }
            public Edge_Media_To_Hoisted_Comment edge_media_to_hoisted_comment { get; set; }
            public Edge_Media_Preview_Comment edge_media_preview_comment { get; set; }
            public bool comments_disabled { get; set; }
            public bool commenting_disabled_for_viewer { get; set; }
            public int taken_at_timestamp { get; set; }
            public Edge_Media_Preview_Like edge_media_preview_like { get; set; }
            public Edge_Media_To_Sponsor_User edge_media_to_sponsor_user { get; set; }
            public Location location { get; set; }
            public bool viewer_has_liked { get; set; }
            public bool viewer_has_saved { get; set; }
            public bool viewer_has_saved_to_collection { get; set; }
            public bool viewer_in_photo_of_you { get; set; }
            public bool viewer_can_reshare { get; set; }
            public Owner3 owner { get; set; }
            public bool is_ad { get; set; }
            public Edge_Web_Media_To_Related_Media edge_web_media_to_related_media { get; set; }
            public Edge_Related_Profiles edge_related_profiles { get; set; }
        }

        public class Dimensions
        {
            public int height { get; set; }
            public int width { get; set; }
        }

        public class Edge_Media_To_Tagged_User
        {
            public Edge[] edges { get; set; }
        }

        public class Edge
        {
            public Node node { get; set; }
        }

        public class Node
        {
            public User user { get; set; }
            public float x { get; set; }
            public float y { get; set; }
        }

        public class User
        {
            public string full_name { get; set; }
            public string id { get; set; }
            public bool is_verified { get; set; }
            public string profile_pic_url { get; set; }
            public string username { get; set; }
        }

        public class Edge_Media_To_Caption
        {
            public Edge1[] edges { get; set; }
        }

        public class Edge1
        {
            public Node1 node { get; set; }
        }

        public class Node1
        {
            public string text { get; set; }
        }

        public class Edge_Media_To_Parent_Comment
        {
            public int count { get; set; }
            public Page_Info page_info { get; set; }
            public Edge2[] edges { get; set; }
        }

        public class Page_Info
        {
            public bool has_next_page { get; set; }
            public object end_cursor { get; set; }
        }

        public class Edge2
        {
            public Node2 node { get; set; }
        }

        public class Node2
        {
            public string id { get; set; }
            public string text { get; set; }
            public int created_at { get; set; }
            public bool did_report_as_spam { get; set; }
            public Owner owner { get; set; }
            public bool viewer_has_liked { get; set; }
            public Edge_Liked_By edge_liked_by { get; set; }
            public bool is_restricted_pending { get; set; }
            public Edge_Threaded_Comments edge_threaded_comments { get; set; }
        }

        public class Owner
        {
            public string id { get; set; }
            public bool is_verified { get; set; }
            public string profile_pic_url { get; set; }
            public string username { get; set; }
        }

        public class Edge_Liked_By
        {
            public int count { get; set; }
        }

        public class Edge_Threaded_Comments
        {
            public int count { get; set; }
            public Page_Info1 page_info { get; set; }
            public Edge3[] edges { get; set; }
        }

        public class Page_Info1
        {
            public bool has_next_page { get; set; }
            public object end_cursor { get; set; }
        }

        public class Edge3
        {
            public Node3 node { get; set; }
        }

        public class Node3
        {
            public string id { get; set; }
            public string text { get; set; }
            public int created_at { get; set; }
            public bool did_report_as_spam { get; set; }
            public Owner1 owner { get; set; }
            public bool viewer_has_liked { get; set; }
            public Edge_Liked_By1 edge_liked_by { get; set; }
            public bool is_restricted_pending { get; set; }
        }

        public class Owner1
        {
            public string id { get; set; }
            public bool is_verified { get; set; }
            public string profile_pic_url { get; set; }
            public string username { get; set; }
        }

        public class Edge_Liked_By1
        {
            public int count { get; set; }
        }

        public class Edge_Media_To_Hoisted_Comment
        {
            public object[] edges { get; set; }
        }

        public class Edge_Media_Preview_Comment
        {
            public int count { get; set; }
            public Edge4[] edges { get; set; }
        }

        public class Edge4
        {
            public Node4 node { get; set; }
        }

        public class Node4
        {
            public string id { get; set; }
            public string text { get; set; }
            public int created_at { get; set; }
            public bool did_report_as_spam { get; set; }
            public Owner2 owner { get; set; }
            public bool viewer_has_liked { get; set; }
            public Edge_Liked_By2 edge_liked_by { get; set; }
            public bool is_restricted_pending { get; set; }
        }

        public class Owner2
        {
            public string id { get; set; }
            public bool is_verified { get; set; }
            public string profile_pic_url { get; set; }
            public string username { get; set; }
        }

        public class Edge_Liked_By2
        {
            public int count { get; set; }
        }

        public class Edge_Media_Preview_Like
        {
            public int count { get; set; }
            public object[] edges { get; set; }
        }

        public class Edge_Media_To_Sponsor_User
        {
            public object[] edges { get; set; }
        }

        public class Location
        {
            public string id { get; set; }
            public bool has_public_page { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string address_json { get; set; }
        }

        public class Owner3
        {
            public string id { get; set; }
            public bool is_verified { get; set; }
            public string profile_pic_url { get; set; }
            public string username { get; set; }
            public bool blocked_by_viewer { get; set; }
            public bool restricted_by_viewer { get; set; }
            public bool followed_by_viewer { get; set; }
            public string full_name { get; set; }
            public bool has_blocked_viewer { get; set; }
            public bool is_private { get; set; }
            public bool is_unpublished { get; set; }
            public bool requested_by_viewer { get; set; }
            public bool pass_tiering_recommendation { get; set; }
            public Edge_Owner_To_Timeline_Media edge_owner_to_timeline_media { get; set; }
            public Edge_Followed_By edge_followed_by { get; set; }
        }

        public class Edge_Owner_To_Timeline_Media
        {
            public int count { get; set; }
        }

        public class Edge_Followed_By
        {
            public int count { get; set; }
        }

        public class Edge_Web_Media_To_Related_Media
        {
            public object[] edges { get; set; }
        }

        public class Edge_Related_Profiles
        {
            public object[] edges { get; set; }
        }

        public class Display_Resources
        {
            public string src { get; set; }
            public int config_width { get; set; }
            public int config_height { get; set; }
        }

    }
}
