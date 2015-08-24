using System;
using System.Collections;
//XmlRpc is included to catch XmlRpcExceptions
using CookComputing.XmlRpc;

using MailChimp;

	class Test {
		private string apikey;

		public ApiWrapper api;

		public Test(string apikey){
			this.apikey =  apikey;
			this.api = new ApiWrapper();
			this.api.setCurrentApiKey(this.apikey);
		}

		public void show_list_details(){
			Console.WriteLine("Connecting to: " + this.api.getUrl());
			MCList[] lists = this.api.lists();
			foreach(MCList list in lists){
				Console.WriteLine("\tID: "+ list.id+" - " + list.name);
				Console.WriteLine("\tList Size: "+ list.member_count);
				MCMergeVar[] vars = this.api.listMergeVars(list.id);
				Console.WriteLine("\tMerge Vars:");
				foreach(MCMergeVar var in vars){
					Console.WriteLine("\t\t"+var.name+" - "+var.tag + " - " + var.req);
				}
				Console.WriteLine("\tInterest Groups:");
				try {
					MCInterestGroups ig = this.api.listInterestGroups(list.id);
					Console.WriteLine("\t\t"+ig.name+" - " + ig.form_field);
					foreach(string group in ig.groups){
						Console.WriteLine("\t\t\t"+group);
					}
				}catch (Exception){
					Console.WriteLine("\t\tInterest Groups not or not configured.");
				}
				Console.WriteLine("\n");
			}
		}//show_list_details
		
		public void list_mergevar_add(){
    		string list_id = "YOUR_LIST_ID";
    		this.api.listMergeVarAdd(list_id, "TESTVAR","My Test Var", false);
		}

		public void list_mergevar_del(){
    		string list_id = "YOUR_LIST_ID";
    		this.api.listMergeVarDel(list_id, "TESTVAR");
		}

		public void list_interestgroup_add(){
    		string list_id = "YOUR_LIST_ID";
    		this.api.listInterestGroupAdd(list_id, "My Group");
		}

		public void list_interestgroup_del(){
    		string list_id = "YOUR_LIST_ID";
    		this.api.listInterestGroupDel(list_id, "My Group");
		}
		
		public void show_member_info(){
		
			string list_id = "YOUR_LIST_ID";
			string email = "INVALID01@mailchimp.com";
			MCMemberInfo info = this.api.listMemberInfo(list_id, email);
			Console.WriteLine("\t"+info.email+" - " + info.email_type);
			Console.WriteLine("\t"+info.status+" - " + info.timestamp);
			foreach(MCMergeVar var in info.merges){
				Console.WriteLine("\t\t"+var.tag + " - " + var.val);
			}
		}
		public void list_members(){

			string list_id = "YOUR_LIST_ID";
			MCListMember[] members = this.api.listMembers(list_id,"subscribed");

			foreach(MCListMember member in members){
				Console.WriteLine("\t\t"+member.email + " - " + member.timestamp);
			}
		
		}		

		public void list_subscribe(){
			string list_id = "YOUR_LIST_ID";
			string email = "INVALID01@mailchimp.com";
			string email_type = "text";
			MCMergeVar[] merges = new MCMergeVar[3];
			merges[0].tag = "FNAME";
			merges[0].val = "Roger";
			merges[1].tag = "LNAME";
			merges[1].val = "Waters";
			merges[2].tag = "INTERESTS";
			merges[2].val = "Sand";
			
			if ( this.api.listSubscribe( list_id, email, merges, email_type, true) ){
				Console.WriteLine("Woot!");
			} else {
				Console.WriteLine("A failure of epic proportions has occurred!");
			}		
		}
		public void list_unsubscribe(){
			string list_id = "YOUR_LIST_ID";
			string email = "INVALID01@mailchimp.com";
			
			if ( this.api.listUnsubscribe( list_id, email ) ){
				Console.WriteLine("Woot!");
			} else {
				Console.WriteLine("A failure of epic proportions has occurred!");
			}		
		}

		public void list_batch_subscribe(){
		//listBatchSubscribe(string id, MCMemberInfo[] batch, bool double_optin, bool update_existing, bool replace_interests);
			string list_id = "YOUR_LIST_ID";
			MCMemberInfo[] batch = new MCMemberInfo[2];
			batch[0].email = "INVALID01@mailchimp.com";
			batch[0].email_type = "html";
			batch[0].merges = new MCMergeVar[3];
			batch[0].merges[0].tag = "FNAME";
			batch[0].merges[0].val = "David";
			batch[0].merges[1].tag = "LNAME";
			batch[0].merges[1].val = "Gilmour";
			batch[0].merges[2].tag = "INTERESTS";
			batch[0].merges[2].val = "Sand";

			batch[1].email = "api@mailchimp.com";
			batch[1].email_type = "html";
			batch[1].merges = new MCMergeVar[3];
			batch[1].merges[0].tag = "FNAME";
			batch[1].merges[0].val = "Roger";
			batch[1].merges[1].tag = "LNAME";
			batch[1].merges[1].val = "Waters";
			batch[1].merges[2].tag = "INTERESTS";
			batch[1].merges[2].val = "Water";
			
			
			MCBatchResult result = this.api.listBatchSubscribe(list_id, batch, false);
			Console.WriteLine("Success:" + result.success_count);
			Console.WriteLine("Errors:" + result.error_count);
			foreach(MCEmailResult item in result.errors){
				Console.WriteLine("\t [" + item.code + "] " + item.message);
				Console.WriteLine("\t\t email: " + item.row.email + " | " + item.row.merges[0].tag + " = " + item.row.merges[0].val);
			}
		}
		
		public void list_batch_unsubscribe(){
		//  MCBatchUnsubResult listBatchUnsubscribe(string id, MCMemberInfo[] batch, bool delete_member, bool send_goodbye, bool send_notify){
			string list_id = "YOUR_LIST_ID";
			MCMemberInfo[] batch = new MCMemberInfo[2];
			batch[0].email = "INVALID01@mailchimp.com";

			batch[1].email = "api@mailchimp.com";			
			
			MCBatchResult result = this.api.listBatchUnsubscribe(list_id, batch, false,false,false);
			Console.WriteLine("Success:" + result.success_count);
			Console.WriteLine("Errors:" + result.error_count);
			foreach(MCEmailResult item in result.errors){
				Console.WriteLine("\t [" + item.code + "] " + item.message);
				Console.WriteLine("\t\t email: " + item.row.email);
			}
		}

		public void update_member(){
		 // listUpdateMember(string listId, string email_address, MCMergeVar[] merges, string email_type, bool replace_interests){		
			string list_id = "YOUR_LIST_ID";
			string email = "api@mailchimp.com";
			bool replace_interests = false;
			string email_type = "text";
			MCMergeVar[] merges = new MCMergeVar[3];
			merges[0].tag = "FNAME";
			merges[0].val = "David";
			merges[1].tag = "LNAME";
			merges[1].val = "Gilmour";
			merges[2].tag = "INTERESTS";
			merges[2].val = "Sand";
			if (this.api.listUpdateMember(list_id, email, merges, email_type, replace_interests) ){
				Console.WriteLine("Woot!");
			} else {
				Console.WriteLine("A failure of epic proportions has occurred!");
			}
		}

		public void campaigns(){
			MCCampaign[] cs = this.api.campaigns();
			Console.WriteLine("Total Campaigns: "+cs.Length);
			foreach(MCCampaign c in cs){
				Console.WriteLine(c.id + " - " + c.title + " - sent: "+c.emails_sent);
			}
		}

		public void campaign_content(){
			string cid = "YOUR_CAMPAIGN_ID";
			MCCampaignContent c = this.api.campaignContent(cid);
			Console.WriteLine("html: " + c.html);
			Console.WriteLine("text: " + c.text);
		}
		
		public void campaign_folders(){
		
			MCCampaignFolder[] folders = this.api.campaignFolders();
			foreach(MCCampaignFolder f in folders){
				Console.WriteLine(f.folder_id + " - " + f.name);
			}
		}

		public void campaign_templates(){
		
			MCTemplate[] temps = this.api.campaignTemplates();
			foreach(MCTemplate t in temps){
				Console.WriteLine(t.id + " - " + t.name + " - " + t.layout);
			}
		}

		public void campaign_abuse_reports(){
			string cid = "YOUR_CAMPAIGN_ID";
			string[] emails = this.api.campaignAbuseReports(cid);
			foreach(string email in emails){
				Console.WriteLine("* " + email);
			}
		}

		public void campaign_click_stats(){
			string cid = "YOUR_CAMPAIGN_ID";
			MCClickURL[] stats = this.api.campaignClickStats(cid);
			foreach(MCClickURL url in stats){
				Console.WriteLine("* " + url.url + " clicks = " + url.stats.clicks + " , unique = " + url.stats.unique);
			}
		}		

		public void campaign_hard_bounces(){
			string cid = "YOUR_CAMPAIGN_ID";
			string[] emails = this.api.campaignHardBounces(cid);
			foreach(string email in emails){
				Console.WriteLine("* " + email);
			}
		}

		public void campaign_soft_bounces(){
			string cid = "YOUR_CAMPAIGN_ID";
			string[] emails = this.api.campaignSoftBounces(cid);
			foreach(string email in emails){
				Console.WriteLine("* " + email);
			}
		}

		public void campaign_stats(){
			string cid = "YOUR_CAMPAIGN_ID";
			MCCampaignStats stats = this.api.campaignStats(cid);
			Console.WriteLine("hard_bounces = " + stats.hard_bounces);
			Console.WriteLine("soft_bounces = " + stats.soft_bounces);
			Console.WriteLine("opens = " + stats.opens);
			Console.WriteLine("emails_sent = " + stats.emails_sent);			
			Console.WriteLine("syntax_errors = " + stats.syntax_errors);
			Console.WriteLine("unsubscribes = " + stats.unsubscribes);
			Console.WriteLine("abuse_reports = " + stats.abuse_reports);
			Console.WriteLine("forwards = " + stats.forwards);
			Console.WriteLine("forwards_opens = " + stats.forwards_opens);
			Console.WriteLine("last_open = " + stats.last_open);
			Console.WriteLine("unique_opens = " + stats.unique_opens);
			Console.WriteLine("clicks = " + stats.clicks);
			Console.WriteLine("unique_clicks = " + stats.unique_clicks);
			Console.WriteLine("last_click = " + stats.last_click);
			Console.WriteLine("users_who_clicked = " + stats.users_who_clicked);
		}
		
		public void campaign_unsubscribes(){
			string cid = "YOUR_CAMPAIGN_ID";
			string[] emails = this.api.campaignUnsubscribes(cid);
			foreach(string email in emails){
				Console.WriteLine("* " + email);
			}
		}
		
		public void aim_click_detail(){
			string cid = "YOUR_CAMPAIGN_ID";
			MCAIMClickDetail[] details = this.api.campaignClickDetailAIM(cid, "http://www.mailchimp.com");
			foreach(MCAIMClickDetail detail in details){
				Console.WriteLine(detail.email + " - " + detail.clicks);
			}
		}

		public void aim_email_detail(){
			string cid = "YOUR_CAMPAIGN_ID";
			MCAIMEmailDetail[] details = this.api.campaignEmailStatsAIM(cid, "INVALID01@mailchimp.com");
			foreach(MCAIMEmailDetail detail in details){
				Console.WriteLine(detail.action + " - " + detail.url + " - " + detail.timestamp);
			}
		}

		public void aim_email_detail_all(){
			string cid = "YOUR_CAMPAIGN_ID";
			MCAIMEmail[] emails = this.api.campaignEmailStatsAIMAll(cid);
			foreach(MCAIMEmail email in emails){
			    Console.WriteLine("email: "+email.email);
			    foreach(MCAIMEmailDetail detail in email.details){
				    Console.WriteLine(detail.action + " - " + detail.url + " - " + detail.timestamp);
			    }
			}
		}

		public void aim_not_opened(){
			string cid = "YOUR_CAMPAIGN_ID";
			string[] emails = this.api.campaignNotOpenedAIM(cid);
			foreach(string email in emails){
				Console.WriteLine("* " + email);
			}
		}

		public void aim_opened(){
			string cid = "YOUR_CAMPAIGN_ID";
			MCAIMEmailOpen[] opens = this.api.campaignOpenedAIM(cid);
			foreach(MCAIMEmailOpen open in opens){
				Console.WriteLine(open.email + " - " + open.open_count);
			}
		}
		
		public void campaign_create(){
		
			MCCampaignOpts opts = new MCCampaignOpts();
			opts.list_id = "YOUR_LIST_ID";
			opts.subject = "Subject A!";
			opts.from_email = "api@mailchimp.com";
			opts.from_name = "MailChimp!";

			opts.template_id = 3;


			MCCampaignContent content = new MCCampaignContent();
			content.text = "text content rules! *|UNSUB|*";
/**
			content.html_header = "header stuffs";
			content.html_footer = "footer sutffs *|UNSUB|*";
			content.html_main = "main main main main main main";
			content.text = "text content rules! *|UNSUB|*";
**/
			
			opts.tracking = new MCCampaignTracking();
			opts.tracking.opens = true;
			opts.tracking.html_clicks = false;
			opts.tracking.text_clicks = false;
			
			opts.authenticate = false;
			
			opts.analytics = new XmlRpcStruct();
            opts.analytics.Add("google", "XXXXX");
	
			opts.title = "Title B!";
			
			MCCampaignTypeOpts type_opts = new MCCampaignTypeOpts();
			type_opts.url = "http://mailchimp.com/blog/rss/";
			
			string new_id = this.api.campaignCreate("rss",opts,content, new MCSegmentOpts(), type_opts);
			Console.WriteLine("New Campaign ID: " + new_id);

		}
		
		public void campaign_update(){
    		string cid = "YOUR_CAMPAIGN_ID";
		
			MCCampaignContent content = new MCCampaignContent();
			content.html_header = "header stuffs 2";
			content.html_footer = "footer sutffs 2 *|UNSUB|*";
			content.html_main = "main main main main main main 2";
			content.text = "text content rules! 2*|UNSUB|*";
            this.api.campaignUpdate(cid, "content", content);


            this.api.campaignUpdate(cid, "title", "My New Title");
            
            this.api.campaignUpdate(cid, "template_id", 1);
		
		}
		
		public void campaign_schedule(){
			string cid = "YOUR_CAMPAIGN_ID";
			string datetime = "2011-01-01 01:02:03";
			bool success = this.api.campaignSchedule(cid, datetime);
			Console.WriteLine("Scheduled? " + success);
		}
		
		public void campaign_unschedule(){
			string cid = "YOUR_CAMPAIGN_ID";
			bool success = this.api.campaignUnschedule(cid);
			Console.WriteLine("Unscheduled? " + success);
		}
		public void campaign_sendtest(){
			string cid = "YOUR_CAMPAIGN_ID";
			string[] emails = new string[2];
			emails[0] = "api@mailchimp.com";
			emails[1] = "INVALID01@mailchimp.com";
			bool success = this.api.campaignSendTest(cid, emails, "html");
			Console.WriteLine("Test messages sent? " + success);
		}

		public void campaign_send(){
			string cid = "YOUR_CAMPAIGN_ID";
			if (this.api.campaignSendNow(cid)){
                Console.WriteLine("Campaign sent");
            } else {
                //you actually should catch the Exception and do something with it here... this won't work.
                Console.WriteLine("Campaign failed to send");
            }
		}
		
		public void campaign_pause(){
			string cid = "YOUR_CAMPAIGN_ID";
            this.api.campaignPause(cid);
            //As above, if this doesn't work, an exception is thrown.
            Console.WriteLine("Campaign paused");
        }
        
		public void campaign_resume(){
			string cid = "YOUR_CAMPAIGN_ID";
            this.api.campaignResume(cid);
            //As above, if this doesn't work, an exception is thrown.
            Console.WriteLine("Campaign resumed");
        }

		public void campaign_segementtest(){
		    string list_id = "YOUR_LIST_ID";
		    MCSegmentOpts segment_opts = new MCSegmentOpts();
		    segment_opts.match = "any";
		    
		    segment_opts.conditions = new MCSegmentCond[2];
		    segment_opts.conditions[0].op = "like";
		    segment_opts.conditions[0].field = "email";
		    segment_opts.conditions[0].value = "@";
		    segment_opts.conditions[1].op = "like";
		    segment_opts.conditions[1].field = "email";
		    segment_opts.conditions[1].value = "mailchimp";
		    
			int result = this.api.campaignSegmentTest(list_id, segment_opts);
			
            Console.WriteLine("Your segments matched "+result+" members");
        			
		}
		
		public void campaign_replicate(){
			string cid = "YOUR_CAMPAIGN_ID";
            string newcid = "YOUR_CAMPAIGN_ID";
            //As above, if this doesn't work, an exception is thrown.
            Console.WriteLine("Campaign " + cid + " replicated. New Campaign Id: " + newcid);
        }

		public void campaign_delete(){
			string cid = "YOUR_CAMPAIGN_ID";
            if (this.api.campaignDelete(cid)){
                Console.WriteLine("Campaign deleted.");
            } else {
                //You'll pretty much never get this - an exception will generally be thrown instead
                Console.WriteLine("Uh-oh, there was a problem deleting your campaign.");            
            }
        }
		
		public void inlineCss(){
		
		    string html = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"\n"+
		                   "http://www.w3.org/TR/html4/loose.dtd\n" +
                            "<html lang=\"en\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><title>untitled</title><style>#header{width:935px;float:left;background:#fecb66;}</style></head> <body><div id=\"header\"><div id=\"header_content\">&nbsp;</div></div><div id=\"container\"><div id=\"content\"></div></div></body></html>";
            bool strip_css = true;
            string newHtml = this.api.inlineCss(html,strip_css);
			Console.WriteLine("Your in-lined content:\n" + newHtml);
		
		}
		
		public void generateText(){
		
		    string html = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"  \"http://www.w3.org/TR/html4/loose.dtd\"> <html lang=\"en\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><title>untitled</title><style>#header{width:935px;float:left;background:#fecb66;}</style></head><body><div id=\"header\"><div id=\"header_content\">Pink Floyd Rocks</div></div><div id=\"container\"><div id=\"content\"><br/>So did the Pumpkins at Red Rocks!</div></div></body></html>";
            string type="html";
            string newHtml = this.api.generateText(type, html);
			Console.WriteLine("Your text-only content:\n" + newHtml);
		
		}

		public void apikeys(){
            string username = "YOUR_PASSWORD";
            string password = "YOUR_PASSWORD";
		    MCApiKey[] keys = this.api.apikeys(username, password, this.api.getCurrentApiKey(), true);
			foreach(MCApiKey key in keys){
    		    Console.WriteLine("key = " + key.apikey + " - created: " + key.created_at +  " - expired: " + key.expired_at);
		    }
		}

		public void apikey_add(){
            string username = "YOUR_PASSWORD";
            string password = "YOUR_PASSWORD";
		    string newKey = this.api.apikeyAdd(username, password, this.api.getCurrentApiKey());
            Console.WriteLine("Your new apikey = " + newKey);
        }

		public void apikey_expire(){
            string username = "YOUR_PASSWORD";
            string password = "YOUR_PASSWORD";
		    this.api.apikeyExpire(username, password, this.api.getCurrentApiKey());
            Console.WriteLine("Expired Key  = " + this.api.getCurrentApiKey());
        }


		public void ping(){
            string resp = this.api.ping();
			Console.WriteLine("Ping returned:\n" + resp);
		}



        public static void Main(string[] args) {

			Console.WriteLine("\nStarting up....");
			try
			{
				if (args.Length < 1 || args[0] == ""){
					showHelp();
					return;
				}
				if (args.Length > 2){
					showHelp();
					return;
				}
				string testToRun = "";
				if (args.Length == 2){
                    testToRun = args[1].Trim();
                    Console.WriteLine("Running: " + testToRun);
                }
				Test t = new Test(args[0]);
				switch(testToRun){
				    case "show_list_details":       t.show_list_details(); break;
			        case "list_mergevar_add":       t.list_mergevar_add(); break;
			        case "list_mergevar_del":       t.list_mergevar_del(); break;
			        case "list_interestgroup_add":  t.list_interestgroup_add(); break;
			        case "list_interestgroup_del":  t.list_interestgroup_del(); break;
			        case "show_member_info":        t.show_member_info(); break;
			        case "list_members":            t.list_members(); break;
			        case "list_subscribe":          t.list_subscribe(); break;
			        case "list_unsubscribe":        t.list_unsubscribe(); break;
			        case "update_member":           t.update_member(); break;
			        case "list_batch_subscribe":    t.list_batch_subscribe(); break;
			        case "list_batch_unsubscribe":  t.list_batch_unsubscribe(); break;
			        case "campaigns":               t.campaigns(); break;
			        case "campaign_content":        t.campaign_content(); break;
			        case "campaign_folders":        t.campaign_folders(); break;
			        case "campaign_templates":      t.campaign_templates(); break;
			        case "campaign_unsubscribes":   t.campaign_unsubscribes(); break;
			        case "campaign_create":         t.campaign_create(); break;
			        case "campaign_update":         t.campaign_update(); break;
			        case "campaign_schedule":       t.campaign_schedule(); break;
			        case "campaign_unschedule":     t.campaign_unschedule(); break;
			        case "campaign_sendtest":       t.campaign_sendtest(); break;
			        case "campaign_send":           t.campaign_send(); break;
			        case "campaign_resume":         t.campaign_resume(); break;
			        case "campaign_pause":          t.campaign_pause(); break;
			        case "campaign_abuse_reports":  t.campaign_abuse_reports(); break;
			        case "campaign_click_stats":    t.campaign_click_stats(); break;
			        case "campaign_hard_bounces":   t.campaign_hard_bounces(); break;
			        case "campaign_soft_bounces":   t.campaign_soft_bounces(); break;
			        case "campaign_stats":          t.campaign_stats(); break;
			        case "aim_click_detail":        t.aim_click_detail(); break;
			        case "aim_email_detail":        t.aim_email_detail(); break;
			        case "aim_email_detail_all":    t.aim_email_detail_all(); break;
			        case "aim_not_opened":          t.aim_not_opened(); break;
			        case "aim_opened":              t.aim_opened(); break;
			        case "inlineCss":               t.inlineCss(); break;
			        case "generateText":            t.generateText(); break;
			        case "ping":                    t.ping(); break;
			        case "campaign_segementtest":   t.campaign_segementtest(); break;
                    case "apikeys":                 t.apikeys(); break;
                    case "apikey_add":              t.apikey_add(); break;
                    case "apikey_expire":           t.apikey_expire(); break;
				    case "":
                        //List Functions
				        t.show_list_details();
                        //Use show_list_details calls around the next 4 to see the changes
				        //t.list_mergevar_add();
				        //t.list_mergevar_del();
				        //t.list_interestgroup_add();
				        //t.list_interestgroup_del();
				        //t.show_member_info();
				        //t.list_members();
				        //t.list_subscribe();
				        //t.list_unsubscribe();
				        //t.update_member();
				        //t.list_batch_subscribe();
				        //t.list_batch_unsubscribe();
				
				        //Campaign Functions
				        //t.campaigns();
				        //t.campaign_content();
				        //t.campaign_folders();
				        //t.campaign_templates();
				        //t.campaign_unsubscribes();
				        //t.campaign_create();
				        //t.campaign_update();
				        //t.campaign_schedule();
				        //t.campaign_unschedule();
				        //t.campaign_sendtest();
				        //t.campaign_send();
				        //t.campaign_resume();
				        //t.campaign_pause();
                        
                        //Campaign Stats functions
				        //t.campaign_abuse_reports();
				        //t.campaign_click_stats();
				        //t.campaign_hard_bounces();
				        //t.campaign_soft_bounces();
				        //t.campaign_stats();
				        //t.aim_click_detail();
				        //t.aim_email_detail();
				        //t.aim_email_detail_all();
				        //t.aim_not_opened();
				        //t.aim_opened();

                        //Utility Functions
				        //t.inlineCss();
				        //t.generateText();
				        //t.ping();
				        //t.campaign_segementtest();
				
                        //Security Functions
                        //t.apikeys();
                        //t.apikey_add();
                        //t.apikeys();
                        //t.apikey_expire();
                        break;
                     default:
                        Console.WriteLine("Can not run [" + testToRun + "] because it does not exist.");
                        break;                     
                }
                
            }
			catch(XmlRpcFaultException fex)
			{
				Console.WriteLine("Fault Code:   " + fex.FaultCode);
				Console.WriteLine("Fault String: " + fex.FaultString);
			}
			Console.WriteLine("\nFinished.\n");
						

		}//Main()
		
		public static void showHelp(){
			Console.WriteLine("Usage:  test.exe <apikey> [<test_to_run>]\n\nSupported tests:\n");
            Console.WriteLine("[List Functions]");
            Console.WriteLine("	# show_list_details");
            Console.WriteLine("	# list_mergevar_add");
            Console.WriteLine("	# list_mergevar_del");
            Console.WriteLine("	# list_interestgroup_add");
            Console.WriteLine("	# list_interestgroup_del");
            Console.WriteLine("	# show_member_info");
            Console.WriteLine("	# list_members");
            Console.WriteLine("	# list_subscribe");
            Console.WriteLine("	# list_unsubscribe");
            Console.WriteLine("	# update_member");
            Console.WriteLine("	# list_batch_subscribe");
            Console.WriteLine("	# list_batch_unsubscribe");
            Console.WriteLine("\n[Campaign Functions]");
            Console.WriteLine("	# campaigns");
            Console.WriteLine("	# campaign_content");
            Console.WriteLine("	# campaign_folders");
            Console.WriteLine("	# campaign_templates");
            Console.WriteLine("	# campaign_unsubscribes");
            Console.WriteLine("	# campaign_create");
            Console.WriteLine("	# campaign_update");
            Console.WriteLine("	# campaign_schedule");
            Console.WriteLine("	# campaign_unschedule");
            Console.WriteLine("	# campaign_sendtest");
            Console.WriteLine("	# campaign_send");
            Console.WriteLine("	# campaign_resume");
            Console.WriteLine("	# campaign_pause");
            Console.WriteLine("\n[Campaign Stats functions]");
            Console.WriteLine("	# campaign_abuse_reports");
            Console.WriteLine("	# campaign_click_stats");
            Console.WriteLine("	# campaign_hard_bounces");
            Console.WriteLine("	# campaign_soft_bounces");
            Console.WriteLine("	# campaign_stats");
            Console.WriteLine("	# aim_click_detail");
            Console.WriteLine("	# aim_email_detail");
            Console.WriteLine("	# aim_email_detail_all");
            Console.WriteLine("	# aim_not_opened");
            Console.WriteLine("	# aim_opened");
            Console.WriteLine("\n[Utility Functions]");
            Console.WriteLine("	# inlineCss");
            Console.WriteLine("	# generateText");
            Console.WriteLine("	# ping");
            Console.WriteLine("	# campaign_segementtest");
            Console.WriteLine("\n[Security Functions]");
            Console.WriteLine("	# apikeys");
            Console.WriteLine("	# apikey_add");
            Console.WriteLine("	# apikey_expire");
			Console.WriteLine("\nUsage:  test.exe <apikey> [<test_to_run>]");
		}



	} //class Test

