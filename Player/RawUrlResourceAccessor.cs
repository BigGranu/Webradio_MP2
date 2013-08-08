using System;
using System.Linq;
using MediaPortal.Common.ResourceAccess;

namespace Webradio.Player
{
  /// <summary>
  /// Simple <see cref="INetworkResourceAccessor"/> implementation that handles a raw url.
  /// Bound to the <see cref="RawUrlMediaProvider"/>.
  /// </summary>
  public class RawUrlResourceAccessor : INetworkResourceAccessor
  {
    protected string _rawUrl = string.Empty;

    public RawUrlResourceAccessor(string url)
    {
      _rawUrl = url;
    }

    public string URL
    {
      get { return _rawUrl; }
    }

    public ResourcePath CanonicalLocalResourcePath
    {
      get { return ResourcePath.BuildBaseProviderPath(RawUrlMediaProvider.RAW_URL_MEDIA_PROVIDER_ID, RawUrlMediaProvider.ToProviderResourcePath(_rawUrl).Serialize()); }
    }

    public IResourceAccessor Clone()
    {
      return new RawUrlResourceAccessor(_rawUrl);
    }

    public IResourceProvider ParentProvider
    {
      get { return null; }
    }

    public string Path
    {
      get { return ResourcePath.BuildBaseProviderPath(RawUrlMediaProvider.RAW_URL_MEDIA_PROVIDER_ID, _rawUrl).Serialize(); }
    }

    public string ResourceName
    {
      get { return new Uri(_rawUrl).Segments.Last(); }
    }

    public string ResourcePathName
    {
      get { return _rawUrl; }
    }

    public void Dispose()
    {
      // nothing to free
    }
  }
}

