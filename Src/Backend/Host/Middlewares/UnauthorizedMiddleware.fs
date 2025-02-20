﻿namespace MusicPlayerBackend.Host

open System.Net.Mime;
open System.Text.Json;
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Json;
open Microsoft.Extensions.Options
open Microsoft.Extensions.DependencyInjection

[<Sealed>]
type UnauthorizedMiddleware(next: RequestDelegate) =
    member _.InvokeAsync(context: HttpContext) = task {
        do! next.Invoke(context)

        if context.Response.HasStarted = false && context.Response.StatusCode = StatusCodes.Status401Unauthorized then
            context.Response.ContentType <- MediaTypeNames.Application.Json

            let jsonOptions = context.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value
            let response = {| Error = "Unauthorized. Refresh token or authorize." |}

            do! context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions.SerializerOptions))
    }
