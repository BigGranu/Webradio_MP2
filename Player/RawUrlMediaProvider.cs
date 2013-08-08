using System;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.ResourceAccess;

namespace Webradio.Player
{
  public class RawUrlMediaProvider : IBaseResourceProvider
  {
    #region Public constants

    /// <summary>
    /// GUID string for the raw url media provider.
    /// </summary>
    protected const string RAW_URL_MEDIA_PROVIDER_ID_STR = "{96364E39-DFD9-462F-B117-EF0D2E87ACE8}";

    /// <summary>
    /// Raw url media provider GUID.
    /// </summary>
    public static Guid RAW_URL_MEDIA_PROVIDER_ID = new Guid(RAW_URL_MEDIA_PROVIDER_ID_STR);

    #endregion

    #region Protected fields

    protected ResourceProviderMetadata _metadata;

    #endregion

    #region Ctor

    public RawUrlMediaProvider()
    {
      _metadata = new ResourceProviderMetadata(RAW_URL_MEDIA_PROVIDER_ID, "Raw Url mediaprovider", "Provides Access to Raw Uri", true, true);
    }

    #endregion

    #region IBaseResourceProvider Member

    public bool TryCreateResourceAccessor(string path, out IResourceAccessor result)
    {
      result = null;
      if (!IsResource(path))
        return false;
      
      result = new RawUrlResourceAccessor(path);
      return true;
    }

    public ResourcePath ExpandResourcePathFromString(string pathStr)
    {
      if (IsResource(pathStr))
        return new ResourcePath(new[] { new ProviderPathSegment(_metadata.ResourceProviderId, pathStr, true) });
      return null;
    }

    public bool IsResource(string url)
    {
      Uri uri;
      if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out uri))
        return !uri.IsFile;
      return false;
    }

    #endregion

    #region IResourceProvider Member

    public ResourceProviderMetadata Metadata
    {
      get { return _metadata; }
    }

    #endregion

    public static ResourcePath ToProviderResourcePath(string path)
    {
      return ResourcePath.BuildBaseProviderPath(RAW_URL_MEDIA_PROVIDER_ID, path);
    }
  }
}
