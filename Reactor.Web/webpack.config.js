const path = require("path");
 const config = {
    entry: {
        app: "./temp/scripts/app.ts",
        vendor: "./temp/scripts/vendor.ts"
    },
        mode: "development",
    output: {
        path: path.resolve(__dirname, "./wwwroot/assets/scripts"),
        filename: "[name]-bundle.js"
    },
  
    resolve: {
        extensions: [".ts", ".js"]
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                exclude: /(node_modules)/,
                loaders: ["babel-loader", "ts-loader"]
            }
        ]
    }
};
 module.exports = config;
