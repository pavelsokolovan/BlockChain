using System.Net.PeerToPeer;

namespace P2P
{
    class PeerEntry
    {
        public PeerName PeerName { get; set; }
        public IP2PService ServiceProxy { get; set; }
        public string DisplayString { get; set; }
    }
}
