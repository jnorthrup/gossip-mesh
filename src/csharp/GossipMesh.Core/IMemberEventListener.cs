using System.Net;

namespace GossipMesh.Core
{
    public interface IMemberEventListener
    {
        void MemberEventCallback(MemberEvent MemberEvent);
    }
}