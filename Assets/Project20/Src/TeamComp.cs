using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class TeamComp : MonoBehaviour
    {
        public Team team;

        public void Set(Team team)
        {
            this.team = team;
        }
        public void SetFromCmsEntity(CmsEntity cmsEntity)
        {
            Vars.teams.GetTeam(cmsEntity.GetComponent<CmsTeamComp>().team.GetCmsEntity());
        }
    }
}