const path = require("path");
module.exports = {
    entry: {
        app: "./temp/scripts/app.ts",
        vendor: "./temp/scripts/vendor.js"
    },
    output: {
        path: path.resolve(__dirname, "./wwwroot/assets/scripts"),
        filename: "[name]-bundle.js"
    },
  
    resolve: {
        // Add `.ts` and `.tsx` as a resolvable extension.
        extensions: [".ts", ".tsx", ".js"]
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                exclude: /(node_modules)/,
                loaders: ["babel-loader", "ts-loader"]
            }
        ]
    },
    mode: "development"
};