using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ResourceApplicationTool.Models;

namespace ResourceApplicationTool.Utils
{
    public class Common
    {
        private static RATV3Entities db = new RATV3Entities();

        public static Guid? CreateImage(HttpPostedFileBase uploadPicture)
        {
            try
            {
                Guid avatarGuid = System.Guid.NewGuid();
                var avatar = new File
                {
                    FileNumber = System.IO.Path.GetFileName(uploadPicture.FileName),
                    FileID = avatarGuid,
                    FileDescription = uploadPicture.ContentType
                };
                using (var reader = new System.IO.BinaryReader(uploadPicture.InputStream))
                {
                    avatar.ItemImage = reader.ReadBytes(uploadPicture.ContentLength);
                }
                db.Files.Add(avatar);
                db.SaveChanges();
                return avatarGuid;
            }
            catch (Exception ex)
            {
                //log
            }
            return null;
        }

        public static void CreateSkillTemplates(Employee emp)
        {
            try
            {

                List<Skill> skills = db.Skills.ToList();

                foreach(Skill skill in skills)
                {
                        SkillLevel sk = db.SkillLevels.Where(x => x.SkillID == skill.SkillID && x.EmployeeID == emp.EmployeeID).FirstOrDefault();
                        if(sk == null)
                        {
                            sk = new SkillLevel();
                            sk.Level = 0;
                            sk.SkillID = skill.SkillID;
                            sk.EmployeeID = emp.EmployeeID;
                        
                            db.SkillLevels.Add(sk);
                        
                        }
                }
                db.SaveChanges();

            }
            catch(Exception ex)
            {
                //error handling
            }
        }
    }
}