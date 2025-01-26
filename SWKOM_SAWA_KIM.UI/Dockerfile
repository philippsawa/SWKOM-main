# Use the official Nginx image
FROM nginx:alpine

# Copy custom Nginx configuration file
COPY SWKOM_SAWA_KIM.UI/nginx.conf /etc/nginx/nginx.conf

# Copy static files into the Nginx HTML directory
COPY SWKOM_SAWA_KIM.UI/ /usr/share/nginx/html