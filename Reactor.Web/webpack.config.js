const path = require("path");
const config = {
    entry: {
        app: "./build/scripts/app.ts"
    },
    mode: "development",
    stats: {
        colors: true,
        reasons: true
    },
    output: {
        path: path.resolve(__dirname, "./wwwroot/assets/scripts"),
        filename: "[name].js"
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
    },
    optimization: {
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /[\\/]node_modules[\\/]/,
                    name: "vendor",
                    chunks: "initial",
                },
            },
        },
        runtimeChunk: {
            name: "manifest",
        },

    },
};
module.exports = config;
