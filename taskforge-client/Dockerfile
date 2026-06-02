# --- Stage 1: Build the React Application ---
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# --- Stage 2: Serve the Static Assets with Nginx ---
FROM nginx:stable-alpine

# Copy our custom nginx config over the default one
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the build output from Stage 1
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]