echo "🔨 Building .NET BE..."
dotnet publish -c Release -o ./publish

echo "📤 Uploading files to VPS..."
scp -r ./publish/* root@14.225.212.59:/var/www/conquer_be/

echo "🔄 Restarting BE service on VPS..."
ssh root@14.225.212.59 << 'EOF'
sudo systemctl daemon-reload
sudo systemctl restart conquer_be
sudo systemctl status conquer_be --no-pager
EOF

echo "✅ Done!"
