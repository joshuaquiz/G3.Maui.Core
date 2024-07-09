﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace G3.Maui.Core.Models;

/// <summary>
/// Represents a mocked HTTP request.
/// </summary>
/// <remarks>
/// This class represents a URL pattern and the HTTP methods that are mocked.
/// </remarks>
/// <param name="UriPattern">A regex for the URL (excluding the domain).</param>
/// <param name="HttpMethodActions">The HTTP methods handled at this URL.</param>
public sealed record MockedHttpRequest(
    Regex UriPattern,
    IReadOnlyCollection<MockedHttpRequestMethodActions> HttpMethodActions);