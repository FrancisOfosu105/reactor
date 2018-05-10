const path = require('path');
module.exports = {
    mode: "development",
    entry: {
        app: "./temp/scripts/app.ts"
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
                loaders: ['babel-loader', 'ts-loader']
            }
        ]
/*
        rules: [
            {   
                test: /\.js$/,
                exclude: /(node_modules)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['env']
                    }
                }
            },
            { test: /\.tsx?$/, loader: "ts-loader" }
        ]
*/
    }
};