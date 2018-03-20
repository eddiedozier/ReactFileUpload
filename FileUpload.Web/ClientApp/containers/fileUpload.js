import React, { Component } from 'react';
import LayoutContentWrapper from '../components/utility/layoutWrapper';
import LayoutContent from '../components/utility/layoutContent';
import PageHeader from '../components/utility/pageHeader';
import Box from '../components/utility/box';
import LayoutWrapper from '../components/utility/layoutWrapper';
import ContentHolder from '../components/utility/contentHolder';
import { Upload, Icon, Modal, Row, Col  } from 'antd';

import axios from 'axios';

class FileUpload extends React.Component {
      constructor(props){
        super(props);
        // this.state = {
        //   previewVisible: false,
        //   previewImage: '',
        //   fileList: []
        // };
        this.state = {
          files: [],
          tags: [],
          tagsInfo:[],
          categories: [],
          currentFileId: 0,
          tagId: 0,
          progress: 0,
          done: false,
          fileList: []
      };
        this.url = "api/file/";
      }

  handleCancel = () => this.setState({ previewVisible: false })

  handlePreview = (file) => {
    this.setState({
      previewImage: file.url || file.thumbUrl,
      previewVisible: true,
    });
  }

  handleChange = ({ fileList }) => this.setState({ fileList })

  uploadfileHandler = (event) => {
    console.log(event.target.files)
    this.setState({done:true});
    const files = event.target.files; 
    const numOfFiles = event.target.files.length;
    let partOfUpload = Math.round(100 / numOfFiles);
    let progress = 0;
    let config = {
        onUploadProgress: () => {
          this.setState({progress: progress});
        }
      };
    for(let i = 0; i < files.length ; i++){
        var formdata = new FormData();
        
        formdata.append("file", files[i]);
        const uploadSuccess = (resp) => {
            progress += partOfUpload;
            this.setState({progress: progress});
            this.setState({progress: 100});
            
        }
        axios.post(this.url + "upload/", formdata, config)
            .then(uploadSuccess, err => err);
    }
}

  render() {
    const { previewVisible, previewImage, fileList } = this.state;
    const uploadButton = (
      <div>
        <Icon type="plus" />
        <div className="ant-upload-text">Upload</div>
      </div>
    );
    return (
      <LayoutWrapper>
        <PageHeader>File Upload</PageHeader>
        <Box>
          <ContentHolder>
            <Row>
              <Col span={12} offset={10}>
                <h2>Upload Your Files</h2>
              </Col>
            </Row>
            <Row>
              <input type="file" name="file-2" id="file-2" onChange={this.uploadfileHandler} multiple />
              <Col span={11} offset={7}>
                <Upload
                multiple={true}
                action="//jsonplaceholder.typicode.com/posts/"
                listType="picture-card"
                fileList={fileList}
                onPreview={this.handlePreview}
                onChange={this.handleChange}
                >
                {fileList.length >= 5 ? null : uploadButton}
                </Upload>
                <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                  <img alt="example" style={{ width: '100%' }} src={previewImage} />
                </Modal>
              </Col>
            </Row>
            
          </ContentHolder>
        </Box>
      </LayoutWrapper>
    );
  }
}

export default FileUpload;