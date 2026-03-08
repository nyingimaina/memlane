# --- Stage 1: Build Frontend ---
FROM node:18-alpine AS frontend-build
WORKDIR /app/front-end
COPY front-end/package*.json ./
RUN npm install
COPY front-end/ ./
RUN npm run build

# --- Stage 2: Build Backend ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /app
COPY back-end/Memlane.Api/Memlane.Api.csproj ./back-end/Memlane.Api/
RUN dotnet restore ./back-end/Memlane.Api/Memlane.Api.csproj
COPY back-end/Memlane.Api/ ./back-end/Memlane.Api/
WORKDIR /app/back-end/Memlane.Api
RUN dotnet publish -c Release -o /app/publish

# --- Stage 3: Final Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=backend-build /app/publish .
# Copy static frontend to wwwroot
COPY --from=frontend-build /app/front-end/out ./wwwroot

# Create a directory for data persistence
RUN mkdir /data
ENV ConnectionStrings__DefaultConnection="Data Source=/data/memlane.db"

EXPOSE 5237
ENTRYPOINT ["dotnet", "Memlane.Api.dll"]
