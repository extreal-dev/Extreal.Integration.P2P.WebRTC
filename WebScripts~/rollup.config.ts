import typescript from "@rollup/plugin-typescript";
import terser from "@rollup/plugin-terser";
import { nodeResolve } from "@rollup/plugin-node-resolve";
import { RollupOptions } from "rollup";

const isProd = process.env.BUILD === "production";

const config: RollupOptions = {
    input: "src/index.ts",
    output: {
        file: "dist/index.js",
        format: "es",
        plugins: isProd ? [terser()] : [],
        globals: {},
    },
    plugins: [
        typescript({
            declaration: true,
            declarationDir: "types",
            exclude: "rollup.config.ts"
        }),
        nodeResolve({
            browser: true,
        }),
    ],
};

export default config;
